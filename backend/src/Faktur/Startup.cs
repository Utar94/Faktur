using Faktur.EntityFrameworkCore.Relational;
using Faktur.EntityFrameworkCore.SqlServer;
using Faktur.Extensions;
using Faktur.Infrastructure;
using Faktur.Settings;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;

namespace Faktur;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;
  private readonly bool _enableOpenApi;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
    _enableOpenApi = configuration.GetValue<bool>("EnableOpenApi");
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    CorsSettings corsSettings = _configuration.GetSection("Cors").Get<CorsSettings>() ?? new();
    services.AddSingleton(corsSettings);
    services.AddCors(corsSettings);

    // TODO(fpion): Authentication

    // TODO(fpion): Authorization

    // TODO(fpion): Session

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddFakturWithEntityFrameworkCoreSqlServer(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<FakturContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }

    //services.AddDistributedMemoryCache(); // TODO(fpion): Session
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (_enableOpenApi)
    {
      builder.UseOpenApi();
    }

    builder.UseHttpsRedirection();
    builder.UseCors();
    //builder.UseSession(); // TODO(fpion): Session
    //builder.UseMiddleware<RenewSession>(); // TODO(fpion): Session
    //builder.UseAuthentication(); // TODO(fpion): Authentication
    //builder.UseAuthorization(); // TODO(fpion): Authorization

    if (builder is WebApplication application)
    {
      application.MapControllers();
    }
  }
}
