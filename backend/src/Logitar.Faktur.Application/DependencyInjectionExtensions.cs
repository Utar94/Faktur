using Logitar.Faktur.Application.Articles;
using Logitar.Faktur.Contracts.Articles;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Faktur.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarFakturApplication(this IServiceCollection services)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddApplicationServices()
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
  }

  private static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    return services.AddTransient<IArticleService, ArticleService>();
  }
}
