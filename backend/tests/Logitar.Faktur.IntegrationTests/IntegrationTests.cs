using Bogus;
using Logitar.Data;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Faktur.Application;
using Logitar.Faktur.EntityFrameworkCore.Relational;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Faktur.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Faktur;

public abstract class IntegrationTests : IAsyncLifetime
{
  protected IApplicationContext ApplicationContext { get; }
  protected Faker Faker { get; } = new();

  protected IServiceProvider ServiceProvider { get; }

  protected EventContext EventContext { get; }
  protected FakturContext FakturContext { get; }

  protected IntegrationTests()
  {
    ApplicationContext = new TestApplicationContext(Faker.Person);

    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    ServiceCollection services = new();
    services.AddMemoryCache();
    services.AddSingleton(configuration);
    services.AddSingleton(ApplicationContext);

    string connectionString;
    DatabaseProvider databaseProvider = configuration.GetValue<DatabaseProvider?>("DatabaseProvider")
      ?? DatabaseProvider.EntityFrameworkCorePostgreSQL;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        connectionString = (configuration.GetValue<string>("SQLCONNSTR_Faktur") ?? string.Empty)
          .Replace("{database}", GetType().Name);
        services.AddLogitarFakturWithEntityFrameworkCoreSqlServer(connectionString);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }

    ServiceProvider = services.BuildServiceProvider();

    EventContext = ServiceProvider.GetRequiredService<EventContext>();
    FakturContext = ServiceProvider.GetRequiredService<FakturContext>();
  }

  public virtual async Task InitializeAsync()
  {
    await EventContext.Database.MigrateAsync();
    await FakturContext.Database.MigrateAsync();

    ISqlHelper sqlHelper = ServiceProvider.GetRequiredService<ISqlHelper>();
    TableId[] tables = new[] { Db.Articles.Table, Db.Actors.Table, Db.Events.Table };
    foreach (TableId table in tables)
    {
      ICommand command = sqlHelper.DeleteFrom(table).Build();
      await FakturContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    ActorEntity actor = new(ApplicationContext.Actor);
    FakturContext.Actors.Add(actor);
    await FakturContext.SaveChangesAsync();
  }
  public virtual Task DisposeAsync() => Task.CompletedTask;

  protected virtual DateTime AsUniversalTime(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
  protected virtual void AssertAreNear(DateTime expected, DateTime actual)
  {
    Assert.Equal(expected.ToUniversalTime(), actual.ToUniversalTime(), TimeSpan.FromSeconds(2));
  }
  protected virtual void AssertIsNear(DateTime value)
  {
    AssertAreNear(DateTime.UtcNow, value.ToUniversalTime());
  }
}
