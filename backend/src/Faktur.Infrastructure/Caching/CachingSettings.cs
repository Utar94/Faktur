namespace Faktur.Infrastructure.Caching;

internal record CachingSettings
{
  public TimeSpan? ActorLifetime { get; set; }
}
