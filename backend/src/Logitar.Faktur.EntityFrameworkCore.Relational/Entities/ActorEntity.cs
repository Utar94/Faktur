using Logitar.Faktur.Contracts.Actors;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Entities;

internal class ActorEntity : Entity
{
  public int ActorId { get; private set; }

  public string Id { get; private set; } = string.Empty;
  public ActorType Type { get; private set; }
  public bool IsDeleted { get; private set; }

  public string DisplayName { get; private set; } = string.Empty;
  public string? EmailAddress { get; private set; }
  public string? PictureUrl { get; private set; }

  public ActorEntity(Actor actor)
  {
    Id = actor.Id;
    Type = actor.Type;
    IsDeleted = actor.IsDeleted;

    DisplayName = actor.DisplayName;
    EmailAddress = actor.EmailAddress;
    PictureUrl = actor.PictureUrl;
  }

  private ActorEntity()
  {
  }

  public Actor ToActor() => new Mapper().ToActor(this);
}
