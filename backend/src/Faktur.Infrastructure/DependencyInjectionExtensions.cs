using Faktur.Application;
using Faktur.Application.Caching;
using Faktur.Infrastructure.Caching;
using Faktur.Infrastructure.Converters;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddFakturInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    return services
      .AddLogitarEventSourcingInfrastructure()
      .AddLogitarPortalClient(configuration)
      .AddFakturApplication()
      .AddMemoryCache()
      .AddSingleton(InitializeCachingSettings)
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer>(_ => new EventSerializer(GetFakturJsonConverters()))
      .AddTransient<IEventBus, EventBus>();
  }

  private static IEnumerable<JsonConverter> GetFakturJsonConverters() =>
  [
    new BannerIdConverter(),
    new DescriptionConverter(),
    new DisplayNameConverter()
  ];

  private static CachingSettings InitializeCachingSettings(IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection("Caching").Get<CachingSettings>() ?? new();
  }
}
