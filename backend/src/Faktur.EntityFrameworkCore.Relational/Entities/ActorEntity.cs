using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;

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

  public ActorEntity(User user)
  {
    Id = new ActorId(user.Id).Value;
    Type = ActorType.User;

    Update(user);
  }

  private ActorEntity()
  {
  }

  public void Update(User user)
  {
    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;
  }
}
