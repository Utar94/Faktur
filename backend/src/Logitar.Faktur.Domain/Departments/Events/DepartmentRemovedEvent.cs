using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Faktur.Domain.Departments.Events;

public record DepartmentRemovedEvent : DomainEvent, INotification // TODO(fpion): serialization
{
  public DepartmentNumberUnit Number { get; }

  public DepartmentRemovedEvent(ActorId actorId, DepartmentNumberUnit number)
  {
    ActorId = actorId;
    Number = number;
  }
}
