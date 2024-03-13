using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Stores.Events;

public record StoreDepartmentDeletedEvent : DomainEvent, INotification
{
  public NumberUnit Number { get; }

  public StoreDepartmentDeletedEvent(NumberUnit number, ActorId actorId)
  {
    Number = number;
    ActorId = actorId;
  }
}
