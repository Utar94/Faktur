using Faktur.Domain.Stores.Events;
using Logitar.EventSourcing;

namespace Faktur.EntityFrameworkCore.Relational.Entities;

internal class DepartmentEntity
{
  public int DepartmentId { get; private set; }

  public StoreEntity? Store { get; private set; }
  public int StoreId { get; private set; }

  public string Number { get; private set; } = string.Empty;
  public string NumberNormalized
  {
    get => Number.ToUpper();
    private set { }
  }
  public string DisplayName { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public string CreatedBy { get; private set; } = ActorId.DefaultValue;
  public DateTime CreatedOn { get; private set; }
  public string UpdatedBy { get; private set; } = ActorId.DefaultValue;
  public DateTime UpdatedOn { get; private set; }

  public List<ProductEntity> Products { get; private set; } = [];

  public DepartmentEntity(StoreEntity store, StoreDepartmentSavedEvent @event)
  {
    Store = store;
    StoreId = store.StoreId;

    Number = @event.Number.Value;

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.ToUniversalTime();

    Update(@event);
  }

  private DepartmentEntity()
  {
  }

  public IEnumerable<ActorId> GetActorIds(bool includeStore)
  {
    List<ActorId> actorIds = [new(CreatedBy), new(UpdatedBy)];
    if (includeStore && Store != null)
    {
      actorIds.AddRange(Store.GetActorIds(includeDepartments: false));
    }
    return actorIds.AsReadOnly();
  }

  public void Update(StoreDepartmentSavedEvent @event)
  {
    DisplayName = @event.Department.DisplayName.Value;
    Description = @event.Department.Description?.Value;

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.ToUniversalTime();
  }

  public override bool Equals(object? obj) => obj is DepartmentEntity department && department.StoreId == StoreId && department.NumberNormalized == NumberNormalized;
  public override int GetHashCode() => HashCode.Combine(GetType(), StoreId, NumberNormalized);
  public override string ToString() => $"{GetType()} (StoreId={StoreId}, Number={NumberNormalized})";
}
