using Faktur.Application;
using Faktur.Application.Caching;
using Faktur.Infrastructure.Caching;
using Faktur.Infrastructure.Converters;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Faktur.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddFakturInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingInfrastructure()
      .AddFakturApplication()
      .AddMemoryCache()
      .AddSingleton(InitializeCachingSettings)
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer>(_ => new EventSerializer(GetFakturJsonConverters()))
      .AddTransient<IEventBus, EventBus>();
  }

  private static IEnumerable<JsonConverter> GetFakturJsonConverters() =>
  [
    new ArticleIdConverter(),
    new BannerIdConverter(),
    new DescriptionConverter(),
    new DisplayNameConverter(),
    new FlagsConverter(),
    new GtinConverter(),
    new NumberConverter(),
    new ProductIdConverter(),
    new ReceiptIdConverter(),
    new SkuConverter(),
    new StoreIdConverter(),
    new TaxCodeConverter(),
    new TaxIdConverter()
  ];

  private static CachingSettings InitializeCachingSettings(IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection("Caching").Get<CachingSettings>() ?? new();
  }
}
