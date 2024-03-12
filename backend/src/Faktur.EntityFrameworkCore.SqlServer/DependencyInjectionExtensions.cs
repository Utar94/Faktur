using Faktur.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.EntityFrameworkCore.SqlServer;

public static class DependencyInjectionExtensions
{
  private const string ConfigurationKey = "SQLCONNSTR_Faktur";

  public static IServiceCollection AddFakturWithEntityFrameworkCoreSqlServer(this IServiceCollection services, IConfiguration configuration)
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
    return services.AddFakturWithEntityFrameworkCoreSqlServer(connectionString.Trim());
  }
  public static IServiceCollection AddFakturWithEntityFrameworkCoreSqlServer(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<FakturContext>(options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Faktur.EntityFrameworkCore.SqlServer")))
      .AddLogitarEventSourcingWithEntityFrameworkCoreSqlServer(connectionString)
      .AddFakturWithEntityFrameworkCoreRelational()
      .AddSingleton<ISearchHelper, SqlServerSearchHelper>()
      .AddSingleton<ISqlHelper, SqlServerHelper>();
  }
}
