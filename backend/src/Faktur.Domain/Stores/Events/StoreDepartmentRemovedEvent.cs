using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Stores.Events;

public record StoreDepartmentRemovedEvent : DomainEvent, INotification
{
  public NumberUnit Number { get; }

  public StoreDepartmentRemovedEvent(NumberUnit number, ActorId actorId)
  {
    Number = number;
    ActorId = actorId;
  }
}
