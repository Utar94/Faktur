using Logitar.EventSourcing.Infrastructure;
using Logitar.Faktur.Application;
using Logitar.Faktur.Application.Caching;
using Logitar.Faktur.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Faktur.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarFakturInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingInfrastructure()
      .AddLogitarFakturApplication()
      .AddScoped<IEventBus, EventBus>()
      .AddSingleton<ICacheService, CacheService>();
  }
}
