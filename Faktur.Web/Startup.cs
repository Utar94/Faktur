namespace Faktur.Web
{
  public class Startup : StartupBase
  {
    public override void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();
      services.AddEndpointsApiExplorer();
      services.AddSwaggerGen();
    }

    public override void Configure(IApplicationBuilder applicationBuilder)
    {
      if (applicationBuilder is WebApplication application)
      {
        if (application.Environment.IsDevelopment())
        {
          application.UseSwagger();
          application.UseSwaggerUI();
        }

        application.UseHttpsRedirection();
        application.UseAuthorization();
        application.MapControllers();
      }
    }
  }
}
