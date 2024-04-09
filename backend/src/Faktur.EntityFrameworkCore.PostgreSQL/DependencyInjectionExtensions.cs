using Faktur.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.EntityFrameworkCore.PostgreSQL;

public static class DependencyInjectionExtensions
{
  private const string ConfigurationKey = "POSTGRESQLCONNSTR_Portal";

  public static IServiceCollection AddFakturWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, IConfiguration configuration)
  {
    string? connectionString = Environment.GetEnvironmentVariable(ConfigurationKey);
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      connectionString = configuration.GetValue<string>(ConfigurationKey);
    }
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new ArgumentException($"The configuration '{ConfigurationKey}' could not be found.", nameof(configuration));
    }
    return services.AddFakturWithEntityFrameworkCorePostgreSQL(connectionString);
  }
  public static IServiceCollection AddFakturWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<FakturContext>(options => options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Faktur.EntityFrameworkCore.PostgreSQL")))
      .AddLogitarEventSourcingWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddFakturWithEntityFrameworkCoreRelational()
      .AddSingleton<ISearchHelper, PostgresSearchHelper>()
      .AddSingleton<ISqlHelper, PostgresHelper>();
  }
}
