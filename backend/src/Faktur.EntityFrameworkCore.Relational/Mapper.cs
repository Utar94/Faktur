using Faktur.Contracts.Banners;
using Faktur.Contracts.Stores;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;

namespace Faktur.EntityFrameworkCore.Relational;

internal class Mapper
{
  private readonly Dictionary<ActorId, Actor> _actors;
  private readonly Actor _system = Actor.System;

  public Mapper()
  {
    _actors = [];
  }
  public Mapper(IEnumerable<Actor> actors) : this()
  {
    foreach (Actor actor in actors)
    {
      ActorId actorId = new(actor.Id);
      _actors[actorId] = actor;
    }
  }

  public static Actor ToActor(ActorEntity source) => new(source.DisplayName)
  {
    Id = new ActorId(source.Id).ToGuid(),
    Type = source.Type,
    IsDeleted = source.IsDeleted,
    EmailAddress = source.EmailAddress,
    PictureUrl = source.PictureUrl
  };

  public Banner ToBanner(BannerEntity source)
  {
    Banner destination = new(source.DisplayName)
    {
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Store ToStore(StoreEntity source)
  {
    Store destination = new(source.DisplayName)
    {
      Number = source.Number,
      Description = source.Description
    };
    if (source.Banner != null)
    {
      destination.Banner = ToBanner(source.Banner);
    }

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Id = new AggregateId(source.AggregateId).ToGuid();
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = AsUniversalTime(source.CreatedOn);
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = AsUniversalTime(source.UpdatedOn);
  }

  private Actor FindActor(string id) => FindActor(new ActorId(id));
  private Actor FindActor(ActorId id) => _actors.TryGetValue(id, out Actor? actor) ? actor : _system;

  private static DateTime AsUniversalTime(DateTime value) => value.Kind switch
  {
    DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
    DateTimeKind.Local => value.ToUniversalTime(),
    _ => value,
  };
}
