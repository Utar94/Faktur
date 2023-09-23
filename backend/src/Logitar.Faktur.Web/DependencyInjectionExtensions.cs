using Logitar.Faktur.Application;
using Logitar.Faktur.Web.Extensions;
using Logitar.Faktur.Web.Filters;
using Logitar.Faktur.Web.Settings;
using System.Text.Json.Serialization;

namespace Logitar.Faktur.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarFakturWeb(this IServiceCollection services, IConfiguration configuration)
  {
    services
     .AddControllersWithViews(options => options.Filters.Add<ExceptionHandlingFilter>())
     .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    CorsSettings corsSettings = configuration.GetSection("Cors").Get<CorsSettings>() ?? new();
    services.AddSingleton(corsSettings);
    services.AddCors(corsSettings);

    services.AddDistributedMemoryCache();
    services.AddMemoryCache();
    services.AddSingleton<IApplicationContext, HttpApplicationContext>();

    return services;
  }
}
