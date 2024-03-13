using Faktur.Domain.Departments;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Stores.Events;

public record StoreDepartmentSavedEvent : DomainEvent, INotification
{
  public NumberUnit Number { get; }
  public DepartmentUnit Department { get; }

  public StoreDepartmentSavedEvent(NumberUnit number, DepartmentUnit department, ActorId actorId)
  {
    Number = number;
    Department = department;
    ActorId = actorId;
  }
}
