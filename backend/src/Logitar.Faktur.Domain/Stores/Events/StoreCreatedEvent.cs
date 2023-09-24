using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Faktur.Domain.Stores.Events;

public record StoreCreatedEvent : DomainEvent, INotification
{
  public string DisplayName { get; init; } = string.Empty;

  public StoreCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }
}
