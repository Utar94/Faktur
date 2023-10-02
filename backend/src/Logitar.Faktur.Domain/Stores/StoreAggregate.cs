using Logitar.EventSourcing;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.Banners;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.Departments.Events;
using Logitar.Faktur.Domain.Products;
using Logitar.Faktur.Domain.Products.Events;
using Logitar.Faktur.Domain.Stores.Events;
using Logitar.Faktur.Domain.ValueObjects;

namespace Logitar.Faktur.Domain.Stores;

public class StoreAggregate : AggregateRoot
{
  private readonly Dictionary<DepartmentNumberUnit, DepartmentUnit> departments = new();
  private readonly Dictionary<ArticleId, ProductUnit> products = new();
  private readonly Dictionary<SkuUnit, ProductUnit> productsBySku = new();

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
        updated.Number = new Modification<StoreNumberUnit>(value);
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
        updated.DisplayName = value;
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
        updated.Description = new Modification<DescriptionUnit>(value);
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

  public IReadOnlyDictionary<DepartmentNumberUnit, DepartmentUnit> Departments => departments.AsReadOnly();
  public IReadOnlyDictionary<ArticleId, ProductUnit> Products => products.AsReadOnly();
  public IReadOnlyDictionary<SkuUnit, ProductUnit> ProductsBySku => productsBySku.AsReadOnly();

  public StoreAggregate(AggregateId id) : base(id)
  {
  }

  public StoreAggregate(DisplayNameUnit displayName, ActorId actorId = default, StoreId? id = null) : base(id?.AggregateId)
  {
    ApplyChange(new StoreCreatedEvent(actorId, displayName));
  }
  protected virtual void Apply(StoreCreatedEvent @event) => displayName = @event.DisplayName;

  public void Delete(ActorId actorId = default) => ApplyChange(new StoreDeletedEvent(actorId));

  public bool RemoveDepartment(DepartmentNumberUnit number, ActorId actorId = default)
  {
    if (!departments.TryGetValue(number, out DepartmentUnit? department))
    {
      return false;
    }

    ApplyChange(new DepartmentRemovedEvent(actorId, department));

    return true;
  }
  protected virtual void Apply(DepartmentRemovedEvent @event) => departments.Remove(@event.Department.Number);

  public bool RemoveProduct(ArticleId articleId, ActorId actorId = default)
  {
    if (!products.TryGetValue(articleId, out ProductUnit? product))
    {
      return false;
    }

    ApplyChange(new ProductRemovedEvent(actorId, product));

    return true;
  }
  protected virtual void Apply(ProductRemovedEvent @event)
  {
    products.Remove(@event.Product.ArticleId);

    if (@event.Product.Sku != null)
    {
      productsBySku.Remove(@event.Product.Sku);
    }
  }

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
    if (!departments.TryGetValue(department.Number, out DepartmentUnit? existingDepartment) || department != existingDepartment)
    {
      ApplyChange(new DepartmentSavedEvent(actorId, department));
    }
  }
  protected virtual void Apply(DepartmentSavedEvent @event) => departments[@event.Department.Number] = @event.Department;

  public void SetProduct(ProductUnit product, ActorId actorId = default)
  {
    if (product.Sku != null && productsBySku.TryGetValue(product.Sku, out ProductUnit? existingProduct) && product.ArticleId != existingProduct.ArticleId)
    {
      throw new SkuAlreadyUsedException(this, product.Sku, nameof(product.Sku));
    }
    else if (!products.TryGetValue(product.ArticleId, out existingProduct) || product != existingProduct)
    {
      ApplyChange(new ProductSavedEvent(actorId, product));
    }
  }
  protected virtual void Apply(ProductSavedEvent @event)
  {
    products[@event.Product.ArticleId] = @event.Product;

    if (@event.Product.Sku != null)
    {
      productsBySku[@event.Product.Sku] = @event.Product;
    }
  }

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
      number = @event.Number.Value;
    }
    if (@event.DisplayName != null)
    {
      displayName = @event.DisplayName;
    }
    if (@event.Description != null)
    {
      description = @event.Description.Value;
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
