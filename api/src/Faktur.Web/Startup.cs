using Faktur.Infrastructure;
using Faktur.Web.Email;
using Faktur.Web.Settings;
using Logitar.AspNetCore.Identity;
using Logitar.Email.SendGrid;
using Logitar.Identity.EntityFrameworkCore;
using Logitar.Validation;
using Logitar.WebApiToolKit;
using Microsoft.EntityFrameworkCore;
using RazorLight;
using System.Reflection;

namespace Faktur.Web
{
  public class Startup : StartupBase
  {
    private readonly ConfigurationOptions options = new();
    private readonly IConfiguration configuration;

    public Startup(IConfiguration configuration)
    {
      this.configuration = configuration;

      options.Filters.Add<IdentityExceptionFilterAttribute>();
    }

    public override void ConfigureServices(IServiceCollection services)
    {
      base.ConfigureServices(services);

      var applicationSettings = configuration.GetSection("Application").Get<ApplicationSettings>() ?? new();
      CompositeValidator.Validate(applicationSettings);
      services.AddSingleton(applicationSettings);

      services.AddWebApiToolKit(configuration, options);

      services.AddDefaultIdentity(configuration)
        .WithEntityFrameworkStores<FakturDbContext>();

      services.AddSingleton<IRazorLightEngine>(_ => new RazorLightEngineBuilder()
        .SetOperatingAssembly(Assembly.GetExecutingAssembly())
        .UseFileSystemProject(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location))
        .UseMemoryCachingProvider()
        .Build()
      );

      services.AddSendGrid();

      services.AddSingleton<IEmailService, EmailService>();

      services.AddDbContext<FakturDbContext>(
        options => options.UseNpgsql(configuration.GetConnectionString(nameof(FakturDbContext)))
      );
    }

    public override void Configure(IApplicationBuilder applicationBuilder)
    {
      if (applicationBuilder is WebApplication application)
      {
        application.UseWebApiToolKit(options);
      }
    }
  }
}
