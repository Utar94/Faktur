using Faktur.Domain.Shared;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Stores.Events;

public record StoreCreatedEvent : DomainEvent, INotification
{
  public DisplayNameUnit DisplayName { get; }

  public StoreCreatedEvent(DisplayNameUnit displayName, ActorId actorId)
  {
    DisplayName = displayName;
    ActorId = actorId;
  }
}
