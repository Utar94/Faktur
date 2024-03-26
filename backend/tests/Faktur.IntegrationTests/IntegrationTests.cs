using Bogus;
using Faktur.EntityFrameworkCore.Relational;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Faktur.EntityFrameworkCore.SqlServer;
using Faktur.Infrastructure;
using Faktur.Infrastructure.Commands;
using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur;

public abstract class IntegrationTests : IAsyncLifetime
{
  private readonly DatabaseProvider _databaseProvider;
  private readonly TestContext _testContext;

  protected Faker Faker { get; } = new();
  protected IServiceProvider ServiceProvider { get; }

  protected FakturContext FakturContext { get; }
  protected IMediator Mediator { get; }

  protected User User => _testContext.User;
  protected Actor Actor => new(User);
  protected ActorId ActorId => new(Actor.Id);

  protected IntegrationTests()
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    ServiceCollection services = new();
    services.AddSingleton(configuration);

    _testContext = TestContext.Create(Faker);
    services.AddSingleton(_testContext);
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TestActivityPipelineBehavior<,>));

    string connectionString;
    _databaseProvider = configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (_databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        connectionString = configuration.GetValue<string>("SQLCONNSTR_Faktur")?.Replace("{Database}", GetType().Name) ?? string.Empty;
        services.AddFakturWithEntityFrameworkCoreSqlServer(connectionString);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(_databaseProvider);
    }

    ServiceProvider = services.BuildServiceProvider();

    FakturContext = ServiceProvider.GetRequiredService<FakturContext>();
    Mediator = ServiceProvider.GetRequiredService<IMediator>();
  }

  public virtual async Task InitializeAsync()
  {
    IPublisher publisher = ServiceProvider.GetRequiredService<IPublisher>();
    await publisher.Publish(new InitializeDatabaseCommand());

    TableId[] tables = [EventDb.Events.Table, FakturDb.Actors.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await FakturContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    ActorEntity actor = new(User);
    FakturContext.Actors.Add(actor);
    await FakturContext.SaveChangesAsync();
  }
  protected IDeleteBuilder CreateDeleteBuilder(TableId table) => _databaseProvider switch
  {
    //DatabaseProvider.EntityFrameworkCorePostgreSQL => PostgresDeleteBuilder.From(table), // TODO(fpion): PostgreSQL
    DatabaseProvider.EntityFrameworkCoreSqlServer => SqlServerDeleteBuilder.From(table),
    _ => throw new DatabaseProviderNotSupportedException(_databaseProvider),
  };

  public virtual Task DisposeAsync() => Task.CompletedTask;
}
