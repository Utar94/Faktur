using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Faktur.Domain.Departments.Events;

public record DepartmentRemovedEvent : DomainEvent, INotification
{
  public string Number { get; init; } = string.Empty;

  public DepartmentRemovedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }
}
