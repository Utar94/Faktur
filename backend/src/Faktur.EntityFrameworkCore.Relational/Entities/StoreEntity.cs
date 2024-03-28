using Faktur.Domain.Stores.Events;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;

namespace Faktur.EntityFrameworkCore.Relational.Entities;

internal class StoreEntity : AggregateEntity
{
  public int StoreId { get; private set; }

  public BannerEntity? Banner { get; private set; }
  public int? BannerId { get; private set; }

  public string? Number { get; private set; }
  public string DisplayName { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public string? AddressStreet { get; private set; }
  public string? AddressLocality { get; private set; }
  public string? AddressPostalCode { get; private set; }
  public string? AddressRegion { get; private set; }
  public string? AddressCountry { get; private set; }
  public string? AddressFormatted { get; private set; }
  public string? AddressVerifiedBy { get; private set; }
  public DateTime? AddressVerifiedOn { get; private set; }
  public bool IsAddressVerified { get; private set; }

  public string? EmailAddress { get; private set; }
  public string? EmailVerifiedBy { get; private set; }
  public DateTime? EmailVerifiedOn { get; private set; }
  public bool IsEmailVerified { get; private set; }

  public string? PhoneCountryCode { get; private set; }
  public string? PhoneNumber { get; private set; }
  public string? PhoneExtension { get; private set; }
  public string? PhoneE164Formatted { get; private set; }
  public string? PhoneVerifiedBy { get; private set; }
  public DateTime? PhoneVerifiedOn { get; private set; }
  public bool IsPhoneVerified { get; private set; }

  public int DepartmentCount { get; private set; }
  public List<DepartmentEntity> Departments { get; private set; } = [];

  public List<ProductEntity> Products { get; private set; } = [];
  public List<ReceiptEntity> Receipts { get; private set; } = [];

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
    if (AddressVerifiedBy != null)
    {
      actorIds.Add(new(AddressVerifiedBy));
    }
    if (EmailVerifiedBy != null)
    {
      actorIds.Add(new(EmailVerifiedBy));
    }
    if (PhoneVerifiedBy != null)
    {
      actorIds.Add(new(PhoneVerifiedBy));
    }
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

    if (@event.Address != null)
    {
      if (@event.Address.Value == null)
      {
        AddressStreet = null;
        AddressLocality = null;
        AddressPostalCode = null;
        AddressRegion = null;
        AddressCountry = null;
        AddressFormatted = null;
      }
      else
      {
        AddressStreet = @event.Address.Value.Street;
        AddressLocality = @event.Address.Value.Locality;
        AddressPostalCode = @event.Address.Value.PostalCode;
        AddressRegion = @event.Address.Value.Region;
        AddressCountry = @event.Address.Value.Country;
        AddressFormatted = @event.Address.Value.Format();
      }

      if (IsAddressVerified && @event.Address.Value?.IsVerified != true)
      {
        AddressVerifiedBy = null;
        AddressVerifiedOn = null;
        IsAddressVerified = false;
      }
      else if (!IsAddressVerified && @event.Address.Value?.IsVerified == true)
      {
        AddressVerifiedBy = @event.ActorId.Value;
        AddressVerifiedOn = @event.OccurredOn.ToUniversalTime();
        IsAddressVerified = true;
      }
    }
    if (@event.Email != null)
    {
      if (@event.Email.Value == null)
      {
        EmailAddress = null;
      }
      else
      {
        EmailAddress = @event.Email.Value.Address;
      }

      if (IsEmailVerified && @event.Email.Value?.IsVerified != true)
      {
        EmailVerifiedBy = null;
        EmailVerifiedOn = null;
        IsEmailVerified = false;
      }
      else if (!IsEmailVerified && @event.Email.Value?.IsVerified == true)
      {
        EmailVerifiedBy = @event.ActorId.Value;
        EmailVerifiedOn = @event.OccurredOn.ToUniversalTime();
        IsEmailVerified = true;
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

      if (IsPhoneVerified && @event.Phone.Value?.IsVerified != true)
      {
        PhoneVerifiedBy = null;
        PhoneVerifiedOn = null;
        IsPhoneVerified = false;
      }
      else if (!IsPhoneVerified && @event.Phone.Value?.IsVerified == true)
      {
        PhoneVerifiedBy = @event.ActorId.Value;
        PhoneVerifiedOn = @event.OccurredOn.ToUniversalTime();
        IsPhoneVerified = true;
      }
    }
  }
}
