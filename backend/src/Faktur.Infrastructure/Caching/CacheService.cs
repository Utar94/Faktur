using Faktur.Application.Caching;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Microsoft.Extensions.Caching.Memory;

namespace Faktur.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private readonly IMemoryCache _memoryCache;
  private readonly CachingSettings _settings;

  public CacheService(IMemoryCache memoryCache, CachingSettings settings)
  {
    _memoryCache = memoryCache;
    _settings = settings;
  }

  public Actor? GetActor(ActorId id) => GetItem<Actor>(GetActorKey(id));
  public void RemoveActor(ActorId id) => SetItem<Actor>(GetActorKey(id), value: null);
  public void SetActor(Actor actor) => SetItem(GetActorKey(actor.Id), actor, _settings.ActorLifetime);
  private static string GetActorKey(Guid id) => GetActorKey(new ActorId(id));
  private static string GetActorKey(ActorId id) => $"Actor.Id:{id}";

  private T? GetItem<T>(object key) => _memoryCache.TryGetValue(key, out object? value) ? (T?)value : default;
  private void SetItem<T>(object key, T? value, TimeSpan? lifetime = null)
  {
    if (value == null)
    {
      _memoryCache.Remove(key);
    }
    else if (lifetime.HasValue)
    {
      _memoryCache.Set(key, value, lifetime.Value);
    }
    else
    {
      _memoryCache.Set(key, value);
    }
  }
}
