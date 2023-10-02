using Logitar.EventSourcing.Infrastructure;
using Logitar.Faktur.Application;
using Logitar.Faktur.Application.Caching;
using Logitar.Faktur.Infrastructure.Caching;
using Logitar.Faktur.Infrastructure.Converters;
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
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer>(_ => new EventSerializer(GetJsonConverters()));
  }

  private static IEnumerable<JsonConverter> GetJsonConverters() => new JsonConverter[]
  {
    new ArticleIdConverter(),
    new BannerIdConverter(),
    new DepartmentNumberUnitConverter(),
    new DescriptionUnitConverter(),
    new DisplayNameUnitConverter(),
    new FlagsUnitConverter(),
    new GtinUnitConverter(),
    new SkuUnitConverter(),
    new StoreIdConverter(),
    new StoreNumberUnitConverter()
  };
}
