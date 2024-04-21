using Faktur.EntityFrameworkCore.SqlServer;
using Faktur.ETL.Worker.Entities;
using Microsoft.EntityFrameworkCore;

namespace Faktur.ETL.Worker;

internal class Startup
{
  private const string ConfigurationKey = "SQLCONNSTR_Legacy";

  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public virtual void ConfigureServices(IServiceCollection services)
  {
    string? legacyConnectionString = Environment.GetEnvironmentVariable(ConfigurationKey);
    if (string.IsNullOrWhiteSpace(legacyConnectionString))
    {
      legacyConnectionString = _configuration.GetValue<string>(ConfigurationKey);
    }
    if (string.IsNullOrWhiteSpace(legacyConnectionString))
    {
      throw new InvalidOperationException($"The configuration '{ConfigurationKey}' could not be found.");
    }

    services.AddDbContext<LegacyContext>(options => options.UseNpgsql(legacyConnectionString));
    services.AddFakturWithEntityFrameworkCoreSqlServer(_configuration);
    services.AddHostedService<Worker>();
    services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
  }
}
