using Logitar.EventSourcing;
using Logitar.Faktur.Application.Actors;
using Logitar.Faktur.Application.Caching;
using Logitar.Faktur.Contracts.Actors;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Actors;

internal class ActorService : IActorService
{
  private readonly DbSet<ActorEntity> actors;
  private readonly ICacheService cacheService;

  public ActorService(ICacheService cacheService, FakturContext context)
  {
    actors = context.Actors;
    this.cacheService = cacheService;
  }


  public async Task<IEnumerable<Actor>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken)
  {
    Dictionary<ActorId, Actor> actors = new();

    HashSet<ActorId> missingIds = new();
    foreach (ActorId id in ids)
    {
      Actor? actor = cacheService.GetActor(id);
      if (actor != null)
      {
        actors[id] = actor;
        cacheService.SetActor(actor);
      }
      else if (id != default)
      {
        missingIds.Add(id);
      }
    }

    if (missingIds.Any())
    {
      HashSet<string> actorIds = missingIds.Select(id => id.Value).ToHashSet();
      ActorEntity[] entities = await this.actors.AsNoTracking()
        .Where(actor => actorIds.Contains(actor.Id))
        .ToArrayAsync(cancellationToken);
      foreach (ActorEntity entity in entities)
      {
        Actor actor = entity.ToActor();
        ActorId id = new(actor.Id);

        actors[id] = entity.ToActor();
        cacheService.SetActor(actor);
      }
    }

    return actors.Values;
  }
}
