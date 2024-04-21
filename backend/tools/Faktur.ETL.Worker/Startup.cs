namespace Faktur.ETL.Worker;

internal class Startup
{
  public virtual void ConfigureServices(IServiceCollection services)
  {
    services.AddHostedService<Worker>();
  }
}
