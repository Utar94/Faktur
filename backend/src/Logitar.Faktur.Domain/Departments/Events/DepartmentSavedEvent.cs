using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Faktur.Domain.Departments.Events;

public record DepartmentSavedEvent : DomainEvent, INotification // TODO(fpion): serialization
{
  public DepartmentUnit Department { get; }

  public DepartmentSavedEvent(ActorId actorId, DepartmentUnit department)
  {
    ActorId = actorId;
    Department = department;
  }
}
