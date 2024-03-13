using Faktur.Domain.Stores.Events;
using Logitar.EventSourcing;

namespace Faktur.EntityFrameworkCore.Relational.Entities;

internal class StoreEntity : AggregateEntity
{
  public int StoreId { get; private set; }

  public BannerEntity? Banner { get; private set; }
  public int? BannerId { get; private set; }

  public string? Number { get; private set; }
  public string DisplayName { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public int DepartmentCount { get; private set; }
  public List<DepartmentEntity> Departments { get; private set; } = [];

  public StoreEntity(StoreCreatedEvent @event) : base(@event)
  {
    DisplayName = @event.DisplayName.Value;
  }

  private StoreEntity() : base()
  {
  }

  public IEnumerable<ActorId> GetActorIds(bool includeDepartments)
  {
    List<ActorId> actorIds = GetActorIds().ToList();
    if (includeDepartments)
    {
      foreach (DepartmentEntity department in Departments)
      {
        actorIds.AddRange(department.GetActorIds(includeStore: false));
      }
    }
    return actorIds.AsReadOnly();
  }

  public void SetBanner(BannerEntity? banner)
  {
    Banner = banner;
    BannerId = banner?.BannerId;
  }

  public void RemoveDepartment(StoreDepartmentRemovedEvent @event)
  {
    Update(@event);

    DepartmentEntity? department = Departments.SingleOrDefault(d => d.Number == @event.Number.Value);
    if (department != null)
    {
      Departments.Remove(department);
    }

    DepartmentCount = Departments.Count;
  }

  public void SetDepartment(StoreDepartmentSavedEvent @event)
  {
    Update(@event);

    DepartmentEntity? department = Departments.SingleOrDefault(d => d.Number == @event.Number.Value);
    if (department == null)
    {
      department = new(this, @event);
      Departments.Add(department);
    }
    else
    {
      department.Update(@event);
    }

    DepartmentCount = Departments.Count;
  }

  public void Update(StoreUpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.Number != null)
    {
      Number = @event.Number.Value?.Value;
    }
    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }
  }
}
