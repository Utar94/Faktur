using Logitar.EventSourcing;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.Banners;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.Departments.Events;
using Logitar.Faktur.Domain.Stores.Events;
using Logitar.Faktur.Domain.ValueObjects;

namespace Logitar.Faktur.Domain.Stores;

public class StoreAggregate : AggregateRoot
{
  private readonly Dictionary<string, DepartmentUnit> departments = new();

  private StoreUpdatedEvent updated = new();

  public new StoreId Id => new(base.Id);

  public BannerId? BannerId { get; private set; }

  private StoreNumberUnit? number = null;
  public StoreNumberUnit? Number
  {
    get => number;
    set
    {
      if (value != number)
      {
        updated.Number = new Modification<string>(value?.Value);
        number = value;
      }
    }
  }
  private DisplayNameUnit? displayName = null;
  public DisplayNameUnit DisplayName
  {
    get => displayName ?? throw new InvalidOperationException($"The {nameof(DisplayName)} has not been initialized.");
    set
    {
      if (value != displayName)
      {
        updated.DisplayName = value.Value;
        displayName = value;
      }
    }
  }
  private DescriptionUnit? description = null;
  public DescriptionUnit? Description
  {
    get => description;
    set
    {
      if (value != description)
      {
        updated.Description = new Modification<string>(value?.Value);
        description = value;
      }
    }
  }

  private AddressUnit? address = null;
  public AddressUnit? Address
  {
    get => address;
    set
    {
      if (value != address)
      {
        updated.Address = new Modification<AddressUnit>(value);
        address = value;
      }
    }
  }
  private PhoneUnit? phone = null;
  public PhoneUnit? Phone
  {
    get => phone;
    set
    {
      if (value != phone)
      {
        updated.Phone = new Modification<PhoneUnit>(value);
        phone = value;
      }
    }
  }

  public IReadOnlyDictionary<string, DepartmentUnit> Departments => departments.AsReadOnly();

  public StoreAggregate(AggregateId id) : base(id)
  {
  }

  public StoreAggregate(DisplayNameUnit displayName, ActorId actorId = default, StoreId? id = null) : base(id?.AggregateId)
  {
    ApplyChange(new StoreCreatedEvent(actorId)
    {
      DisplayName = displayName.Value
    });
  }
  protected virtual void Apply(StoreCreatedEvent @event)
  {
    displayName = new(@event.DisplayName);
  }

  public void Delete(ActorId actorId = default) => ApplyChange(new StoreDeletedEvent(actorId));

  public bool RemoveDepartment(DepartmentNumberUnit number, ActorId actorId = default)
  {
    if (!departments.ContainsKey(number.Value))
    {
      return false;
    }

    ApplyChange(new DepartmentRemovedEvent(actorId)
    {
      Number = number.Value
    });

    return true;
  }
  protected virtual void Apply(DepartmentRemovedEvent @event) => departments.Remove(@event.Number);

  public void SetBanner(BannerAggregate? banner)
  {
    if (banner?.Id != BannerId)
    {
      updated.BannerId = new Modification<AggregateId?>(banner?.Id.AggregateId);
      BannerId = banner?.Id;
    }
  }

  public void SetDepartment(DepartmentUnit department, ActorId actorId = default)
  {
    if (!departments.TryGetValue(department.Number.Value, out DepartmentUnit? existingDepartment) || department != existingDepartment)
    {
      ApplyChange(new DepartmentSavedEvent(actorId)
      {
        Number = department.Number.Value,
        DisplayName = department.DisplayName.Value,
        Description = department.Description?.Value
      });
    }
  }
  protected virtual void Apply(DepartmentSavedEvent @event) => departments[@event.Number] = new DepartmentUnit(
    new DepartmentNumberUnit(@event.Number), new DisplayNameUnit(@event.DisplayName), DescriptionUnit.TryCreate(@event.Description));

  public void Update(ActorId actorId = default)
  {
    if (updated.HasChanges)
    {
      updated.ActorId = actorId;
      ApplyChange(updated);

      updated = new();
    }
  }
  protected virtual void Apply(StoreUpdatedEvent @event)
  {
    if (@event.BannerId != null)
    {
      BannerId = @event.BannerId.Value.HasValue ? new(@event.BannerId.Value.Value) : null;
    }

    if (@event.Number != null)
    {
      number = @event.Number.Value == null ? null : new StoreNumberUnit(@event.Number.Value);
    }
    if (@event.DisplayName != null)
    {
      displayName = @event.DisplayName == null ? null : new DisplayNameUnit(@event.DisplayName);
    }
    if (@event.Description != null)
    {
      description = @event.Description.Value == null ? null : new DescriptionUnit(@event.Description.Value);
    }

    if (@event.Address != null)
    {
      address = @event.Address.Value;
    }
    if (@event.Phone != null)
    {
      phone = @event.Phone.Value;
    }
  }

  public override string ToString() => $"{DisplayName.Value} | {base.ToString()}";
}
