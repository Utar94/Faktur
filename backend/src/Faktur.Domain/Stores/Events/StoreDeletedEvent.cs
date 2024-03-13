using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Stores.Events;

public record StoreDeletedEvent : DomainEvent, INotification
{
  public StoreDeletedEvent(ActorId actorId)
  {
    ActorId = actorId;
    IsDeleted = true;
  }
}
