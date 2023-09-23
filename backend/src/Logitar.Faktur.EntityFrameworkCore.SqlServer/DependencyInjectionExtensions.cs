using Logitar.EventSourcing.EntityFrameworkCore.SqlServer;
using Logitar.Faktur.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Faktur.EntityFrameworkCore.SqlServer;

public static class DependencyInjectionExtensions
{
  private const string ConnectionStringKey = "SQLCONNSTR_Faktur";

  public static IServiceCollection AddLogitarFakturWithEntityFrameworkCoreSqlServer(this IServiceCollection services, IConfiguration configuration)
  {
    string connectionString = configuration.GetValue<string>(ConnectionStringKey)
      ?? throw new ArgumentException($"The configuration key '{ConnectionStringKey}' could not be found.", nameof(configuration));

    return services
      .AddDbContext<FakturContext>(builder => builder.UseSqlServer(connectionString,
        b => b.MigrationsAssembly("Logitar.Faktur.EntityFrameworkCore.SqlServer")
      ))
      .AddLogitarEventSourcingWithEntityFrameworkCoreSqlServer(connectionString)
      .AddLogitarFakturWithEntityFrameworkCoreRelational()
      .AddSingleton<ISqlHelper, SqlServerHelper>();
  }
}
