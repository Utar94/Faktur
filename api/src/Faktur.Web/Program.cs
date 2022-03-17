using Faktur.Infrastructure;
using Faktur.Web;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

WebApplication application = builder.Build();

startup.Configure(application);

if (application.Environment.IsDevelopment())
{
  using IServiceScope scope = application.Services.CreateScope();
  using var context = scope.ServiceProvider.GetRequiredService<FakturDbContext>();
  context.Database.Migrate();
}

application.Run();
