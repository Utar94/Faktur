using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Faktur.Domain.Departments.Events;

public record DepartmentSavedEvent : DomainEvent, INotification
{
  public string Number { get; init; } = string.Empty;
  public string DisplayName { get; init; } = string.Empty;
  public string? Description { get; init; }

  public DepartmentSavedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }
}
