using Logitar.EventSourcing;
using Logitar.Faktur.Domain.ValueObjects;
using MediatR;

namespace Logitar.Faktur.Domain.Stores.Events;

public record StoreCreatedEvent : DomainEvent, INotification
{
  public DisplayNameUnit DisplayName { get; init; }

  public StoreCreatedEvent(ActorId actorId, DisplayNameUnit displayName)
  {
    ActorId = actorId;
    DisplayName = displayName;
  }
}
