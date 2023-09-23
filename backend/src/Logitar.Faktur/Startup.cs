using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Faktur.EntityFrameworkCore.Relational;
using Logitar.Faktur.EntityFrameworkCore.SqlServer;
using Logitar.Faktur.Web;
using Logitar.Faktur.Web.Extensions;

namespace Logitar.Faktur;

internal class Startup : StartupBase
{
  private readonly IConfiguration configuration;
  private readonly bool enableOpenApi;

  public Startup(IConfiguration configuration)
  {
    this.configuration = configuration;
    enableOpenApi = configuration.GetValue<bool>("EnableOpenApi");
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddLogitarFakturWeb(configuration);

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    if (enableOpenApi)
    {
      services.AddOpenApi();
    }

    string connectionString;
    DatabaseProvider databaseProvider = configuration.GetValue<DatabaseProvider?>("DatabaseProvider")
      ?? DatabaseProvider.EntityFrameworkCorePostgreSQL;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        connectionString = configuration.GetValue<string>("SQLCONNSTR_Faktur") ?? string.Empty;
        services.AddLogitarFakturWithEntityFrameworkCoreSqlServer(connectionString);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<FakturContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (enableOpenApi)
    {
      builder.UseOpenApi();
    }

    builder.UseHttpsRedirection();
    builder.UseCors();

    if (builder is WebApplication application)
    {
      application.MapControllers();
      application.MapHealthChecks("/health");
    }
  }
}
