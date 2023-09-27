using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Faktur.Domain.Departments.Events;

public record DepartmentSavedEvent : DomainEvent, INotification
{
  public DepartmentUnit Department { get; init; }

  public DepartmentSavedEvent(ActorId actorId, DepartmentUnit department)
  {
    ActorId = actorId;
    Department = department;
  }
}
