using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Faktur.Domain.Departments.Events;

public record DepartmentRemovedEvent : DomainEvent, INotification
{
  public DepartmentUnit Department { get; init; }

  public DepartmentRemovedEvent(ActorId actorId, DepartmentUnit department)
  {
    ActorId = actorId;
    Department = department;
  }
}
