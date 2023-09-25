using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Departments.Events;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Entities;

internal class DepartmentEntity : Entity
{
  public int DepartmentId { get; private set; }

  public StoreEntity? Store { get; private set; }
  public int StoreId { get; private set; }

  public string Number { get; private set; } = string.Empty;
  public int NumberNormalized { get; private set; }
  public string DisplayName { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public long Version { get; private set; }

  public string CreatedBy { get; private set; } = string.Empty;
  public DateTime CreatedOn { get; private set; }

  public string UpdatedBy { get; private set; } = string.Empty;
  public DateTime UpdatedOn { get; private set; }

  public DepartmentEntity(DepartmentSavedEvent @event, StoreEntity store)
  {
    Store = store;
    StoreId = store.StoreId;

    Number = @event.Number;
    NumberNormalized = int.Parse(@event.Number);

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.ToUniversalTime();

    Update(@event);
  }

  private DepartmentEntity()
  {
  }

  public IEnumerable<ActorId> GetActorIds() => new ActorId[] { new(CreatedBy), new(UpdatedBy) }
    .Concat(Store?.GetActorIds() ?? Enumerable.Empty<ActorId>());

  public void Update(DepartmentSavedEvent @event)
  {
    DisplayName = @event.DisplayName;
    Description = @event.Description;

    Version++;

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.ToUniversalTime();
  }
}
