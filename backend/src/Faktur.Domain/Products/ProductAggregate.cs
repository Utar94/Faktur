using Faktur.Contracts;
using Faktur.Contracts.Products;
using Faktur.Domain.Articles;
using Faktur.Domain.Products.Events;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Logitar.EventSourcing;

namespace Faktur.Domain.Products;

public class ProductAggregate : AggregateRoot
{
  private ProductUpdatedEvent _updatedEvent = new();

  public new ProductId Id => new(base.Id);

  private StoreId? _storeId = null;
  public StoreId StoreId => _storeId ?? throw new InvalidOperationException($"The '{nameof(StoreId)}' has not been initialized yet.");
  private ArticleId? _articleId = null;
  public ArticleId ArticleId => _articleId ?? throw new InvalidOperationException($"The '{nameof(ArticleId)}' has not been initialized yet.");
  private NumberUnit? _departmentNumber = null;
  public NumberUnit? DepartmentNumber
  {
    get => _departmentNumber;
    set
    {
      if (value != _departmentNumber)
      {
        _departmentNumber = value;
        _updatedEvent.DepartmentNumber = new Modification<NumberUnit>(value);
      }
    }
  }

  private SkuUnit? _sku = null;
  public SkuUnit? Sku
  {
    get => _sku;
    set
    {
      if (value != _sku)
      {
        _sku = value;
        _updatedEvent.Sku = new Modification<SkuUnit>(value);
      }
    }
  }
  private DisplayNameUnit? _displayName = null;
  public DisplayNameUnit? DisplayName
  {
    get => _displayName;
    set
    {
      if (value != _displayName)
      {
        _displayName = value;
        _updatedEvent.DisplayName = new Modification<DisplayNameUnit>(value);
      }
    }
  }
  private DescriptionUnit? _description = null;
  public DescriptionUnit? Description
  {
    get => _description;
    set
    {
      if (value != _description)
      {
        _description = value;
        _updatedEvent.Description = new Modification<DescriptionUnit>(value);
      }
    }
  }

  private FlagsUnit? _flags = null;
  public FlagsUnit? Flags
  {
    get => _flags;
    set
    {
      if (value != _flags)
      {
        _flags = value;
        _updatedEvent.Flags = new Modification<FlagsUnit>(value);
      }
    }
  }

  private double? _unitPrice = null;
  public double? UnitPrice
  {
    get => _unitPrice;
    set
    {
      if (value != _unitPrice)
      {
        _unitPrice = value;
        _updatedEvent.UnitPrice = new Modification<double?>(value);
      }
    }
  }
  private UnitType? _unitType = null;
  public UnitType? UnitType
  {
    get => _unitType;
    set
    {
      if (value != _unitType)
      {
        _unitType = value;
        _updatedEvent.UnitType = new Modification<UnitType?>(value);
      }
    }
  }

  public ProductAggregate(AggregateId id) : base(id)
  {
  }

  public ProductAggregate(StoreAggregate store, ArticleAggregate article, ActorId actorId = default, ProductId? id = null)
    : base((id ?? ProductId.NewId()).AggregateId)
  {
    Raise(new ProductCreatedEvent(store.Id, article.Id, actorId));
  }
  protected virtual void Apply(ProductCreatedEvent @event)
  {
    _storeId = @event.StoreId;
    _articleId = @event.ArticleId;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new ProductDeletedEvent(actorId));
    }
  }

  public void Update(ActorId actorId = default)
  {
    if (_updatedEvent.HasChanges)
    {
      _updatedEvent.ActorId = actorId;
      Raise(_updatedEvent);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(ProductUpdatedEvent @event)
  {
    if (@event.DepartmentNumber != null)
    {
      _departmentNumber = @event.DepartmentNumber.Value;
    }

    if (@event.Sku != null)
    {
      _sku = @event.Sku.Value;
    }
    if (@event.DisplayName != null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }

    if (@event.Flags != null)
    {
      _flags = @event.Flags.Value;
    }

    if (@event.UnitPrice != null)
    {
      _unitPrice = @event.UnitPrice.Value;
    }
    if (@event.UnitType != null)
    {
      _unitType = @event.UnitType.Value;
    }
  }
}
