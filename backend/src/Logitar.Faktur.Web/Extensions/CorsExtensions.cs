using Logitar.Faktur.Web.Settings;

namespace Logitar.Faktur.Web.Extensions;

internal static class CorsExtensions
{
  public static IServiceCollection AddCors(this IServiceCollection services, CorsSettings settings)
  {
    services.AddCors(options => options.AddDefaultPolicy(cors =>
    {
      if (settings.AllowAnyOrigin)
      {
        cors.AllowAnyOrigin();
      }
      else
      {
        cors.WithOrigins(settings.AllowedOrigins);
      }

      if (settings.AllowAnyMethod)
      {
        cors.AllowAnyMethod();
      }
      else
      {
        cors.WithMethods(settings.AllowedMethods);
      }

      if (settings.AllowAnyHeader)
      {
        cors.AllowAnyHeader();
      }
      else
      {
        cors.WithHeaders(settings.AllowedHeaders);
      }

      if (settings.AllowCredentials)
      {
        cors.AllowCredentials();
      }
      else
      {
        cors.DisallowCredentials();
      }
    }));

    return services;
  }
}
