using Microsoft.OpenApi.Models;

namespace Logitar.Faktur.Web.Extensions;

public static class OpenApiExtensions
{
  private const string Title = "Faktur API";
  private static readonly Version Version = new(2, 0, 0);

  public static IServiceCollection AddOpenApi(this IServiceCollection services)
  {
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(config =>
    {
      config.SwaggerDoc(name: $"v{Version.Major}", new OpenApiInfo
      {
        Contact = new OpenApiContact
        {
          Email = "francispion@hotmail.com",
          Name = "Francis Pion",
          Url = new Uri("https://github.com/Utar94/Faktur", UriKind.Absolute)
        },
        Description = "Receipt management system.",
        License = new OpenApiLicense
        {
          Name = "Use under MIT",
          Url = new Uri("https://github.com/Utar94/Faktur/blob/main/LICENSE", UriKind.Absolute)
        },
        Title = Title,
        Version = $"v{Version}"
      });
    });

    return services;
  }

  public static void UseOpenApi(this IApplicationBuilder builder)
  {
    builder.UseSwagger();
    builder.UseSwaggerUI(config => config.SwaggerEndpoint(
      url: $"/swagger/v{Version.Major}/swagger.json",
      name: $"{Title} v{Version}"
    ));
  }
}
