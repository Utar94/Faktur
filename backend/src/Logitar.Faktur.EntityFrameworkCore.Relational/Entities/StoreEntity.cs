using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Departments.Events;
using Logitar.Faktur.Domain.Stores;
using Logitar.Faktur.Domain.Stores.Events;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Entities;

internal class StoreEntity : AggregateEntity
{
  public int StoreId { get; private set; }

  public BannerEntity? Banner { get; private set; }
  public int? BannerId { get; private set; }

  public string? Number { get; private set; }
  public int? NumberNormalized { get; private set; }
  public string DisplayName { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public string? AddressStreet { get; private set; }
  public string? AddressLocality { get; private set; }
  public string? AddressRegion { get; private set; }
  public string? AddressPostalCode { get; private set; }
  public string? AddressCountry { get; private set; }
  public string? AddressFormatted { get; private set; }

  public string? PhoneCountryCode { get; private set; }
  public string? PhoneNumber { get; private set; }
  public string? PhoneExtension { get; private set; }
  public string? PhoneE164Formatted { get; private set; }

  public List<DepartmentEntity> Departments { get; private set; } = new();

  public StoreEntity(StoreCreatedEvent @event) : base(@event)
  {
    DisplayName = @event.DisplayName.Value;
  }

  private StoreEntity() : base()
  {
  }

  public override IEnumerable<ActorId> GetActorIds() => GetActorIds(addDepartments: true);
  public IEnumerable<ActorId> GetActorIds(bool addDepartments)
  {
    List<ActorId> ids = new(capacity: 4 + (2 * Departments.Count))
    {
      new ActorId(CreatedBy),
      new ActorId(UpdatedBy)
    };

    if (Banner != null)
    {
      ids.AddRange(Banner.GetActorIds());
    }

    if (addDepartments)
    {
      foreach (DepartmentEntity department in Departments)
      {
        ids.AddRange(department.GetActorIds(addStore: false));
      }
    }

    return ids;
  }

  public void RemoveDepartment(DepartmentRemovedEvent @event)
  {
    Update(@event);

    DepartmentEntity? department = Departments.SingleOrDefault(d => d.NumberNormalized == @event.Department.Number.NormalizedValue);
    if (department != null)
    {
      Departments.Remove(department);
    }
  }

  public void SetDepartment(DepartmentSavedEvent @event)
  {
    Update(@event);

    DepartmentEntity? department = Departments.SingleOrDefault(d => d.NumberNormalized == @event.Department.Number.NormalizedValue);
    if (department == null)
    {
      department = new(@event, this);
      Departments.Add(department);
    }
    else
    {
      department.Update(@event);
    }
  }

  public void Update(StoreUpdatedEvent @event, BannerEntity? banner)
  {
    Update(@event);

    if (@event.BannerId != null)
    {
      Banner = banner;
      BannerId = banner?.BannerId;
    }

    if (@event.Number != null)
    {
      Number = @event.Number.Value?.Value;
      NumberNormalized = @event.Number.Value?.NormalizedValue;
    }
    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Address != null)
    {
      if (@event.Address.Value == null)
      {
        AddressStreet = null;
        AddressLocality = null;
        AddressRegion = null;
        AddressPostalCode = null;
        AddressCountry = null;
        AddressFormatted = null;
      }
      else
      {
        AddressStreet = @event.Address.Value.Street;
        AddressLocality = @event.Address.Value.Locality;
        AddressRegion = @event.Address.Value.Region;
        AddressPostalCode = @event.Address.Value.PostalCode;
        AddressCountry = @event.Address.Value.Country;
        AddressFormatted = PostalAddressHelper.Format(@event.Address.Value);
      }
    }
    if (@event.Phone != null)
    {
      if (@event.Phone.Value == null)
      {
        PhoneCountryCode = null;
        PhoneNumber = null;
        PhoneExtension = null;
        PhoneE164Formatted = null;
      }
      else
      {
        PhoneCountryCode = @event.Phone.Value.CountryCode;
        PhoneNumber = @event.Phone.Value.Number;
        PhoneExtension = @event.Phone.Value.Extension;
        PhoneE164Formatted = @event.Phone.Value.FormatToE164();
      }
    }
  }
}
