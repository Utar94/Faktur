using Faktur.EntityFrameworkCore.PostgreSQL;
using Faktur.EntityFrameworkCore.SqlServer;
using Faktur.Infrastructure;
using Faktur.Relocation.Worker.Settings;
using Microsoft.EntityFrameworkCore;

namespace Faktur.Relocation.Worker;

internal class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddHostedService<Worker>();
    services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

    AddDestination(services);
    AddSource(services);
  }

  private void AddDestination(IServiceCollection services)
  {
    DatabaseSettings settings = _configuration.GetSection("Destination").Get<DatabaseSettings>() ?? new();
    switch (settings.DatabaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        services.AddFakturWithEntityFrameworkCorePostgreSQL(settings.ConnectionString);
        break;
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddFakturWithEntityFrameworkCoreSqlServer(settings.ConnectionString);
        break;
    }
  }

  private void AddSource(IServiceCollection services)
  {
    DatabaseSettings settings = _configuration.GetSection("Source").Get<DatabaseSettings>() ?? new();
    switch (settings.DatabaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        services.AddDbContext<SourceContext>(options => options.UseNpgsql(settings.ConnectionString));
        break;
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddDbContext<SourceContext>(options => options.UseSqlServer(settings.ConnectionString));
        break;
    }
  }
}
