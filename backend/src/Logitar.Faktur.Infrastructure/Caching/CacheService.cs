using Logitar.EventSourcing;
using Logitar.Faktur.Application.Caching;
using Logitar.Faktur.Contracts.Actors;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Faktur.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private readonly IMemoryCache cache;

  public CacheService(IMemoryCache cache)
  {
    this.cache = cache;
  }

  public Actor? GetActor(ActorId id) => GetItem<Actor>(GetActorKey(id));
  public void SetActor(Actor actor) => SetItem(GetActorKey(actor.Id), actor, TimeSpan.FromMinutes(15));
  private static string GetActorKey(string id) => GetActorKey(new ActorId(id));
  private static string GetActorKey(ActorId id) => $"Actor:Id:{id}";

  private T? GetItem<T>(object key) => cache.TryGetValue(key, out T? value) ? value : default;
  private void SetItem(object key, object value, TimeSpan expiration) => cache.Set(key, value, expiration);
}
