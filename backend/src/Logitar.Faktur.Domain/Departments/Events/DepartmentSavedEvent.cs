using Logitar.EventSourcing;
using Logitar.Faktur.Domain.ValueObjects;
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

  public static DepartmentSavedEvent Create(ActorId actorId, DepartmentUnit department) => new(actorId)
  {

  };

  public DepartmentUnit GetDepartment()
  {
    DepartmentNumberUnit number = new(Number);
    DisplayNameUnit displayName = new(DisplayName);
    DescriptionUnit? description = DescriptionUnit.TryCreate(Description);

    return new DepartmentUnit(number, displayName, description);
  }
}
