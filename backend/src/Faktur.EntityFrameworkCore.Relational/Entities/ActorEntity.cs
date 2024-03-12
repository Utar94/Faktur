using Logitar.Portal.Contracts.Actors;

namespace Faktur.EntityFrameworkCore.Relational.Entities;

internal class ActorEntity
{
  public int ActorId { get; private set; }

  public string Id { get; private set; } = Logitar.EventSourcing.ActorId.DefaultValue;
  public ActorType Type { get; private set; }
  public bool IsDeleted { get; private set; }

  public string DisplayName { get; private set; } = string.Empty;
  public string? EmailAddress { get; private set; }
  public string? PictureUrl { get; private set; }

  private ActorEntity()
  {
  }
}
