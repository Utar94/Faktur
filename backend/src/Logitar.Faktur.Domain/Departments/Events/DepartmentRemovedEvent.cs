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

  public static DepartmentRemovedEvent Create(ActorId actorId, DepartmentNumberUnit number) => new(actorId)
  {
    Number = number.Value
  };

  public DepartmentNumberUnit GetNumber() => new(Number);
}
