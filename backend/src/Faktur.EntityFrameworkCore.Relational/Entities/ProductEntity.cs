using Faktur.Contracts.Products;
using Faktur.Domain.Products.Events;

namespace Faktur.EntityFrameworkCore.Relational.Entities;

internal class ProductEntity : AggregateEntity
{
  public int ProductId { get; private set; }

  public StoreEntity? Store { get; private set; }
  public int StoreId { get; private set; }
  public ArticleEntity? Article { get; private set; }
  public int ArticleId { get; private set; }
  public DepartmentEntity? Department { get; private set; }
  public int? DepartmentId { get; private set; }

  public string? Sku { get; private set; }
  public string? SkuNormalized
  {
    get => Sku?.ToUpper();
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string? Flags { get; private set; }

  public decimal? UnitPrice { get; private set; }
  public UnitType? UnitType { get; private set; }

  public List<ReceiptItemEntity> ReceiptItems { get; private set; } = [];

  public ProductEntity(StoreEntity store, ArticleEntity article, ProductCreatedEvent @event) : base(@event)
  {
    Store = store;
    StoreId = store.StoreId;
    Article = article;
    ArticleId = article.ArticleId;
  }

  private ProductEntity()
  {
  }

  public void Update(ProductUpdatedEvent @event, DepartmentEntity? department)
  {
    Update(@event);

    if (@event.DepartmentNumber != null)
    {
      Department = department;
      DepartmentId = department?.DepartmentId;
    }

    if (@event.Sku != null)
    {
      Sku = @event.Sku.Value?.Value;
    }
    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Flags != null)
    {
      Flags = @event.Flags.Value?.Value;
    }

    if (@event.UnitPrice != null)
    {
      UnitPrice = @event.UnitPrice.Value;
    }
    if (@event.UnitType != null)
    {
      UnitType = @event.UnitType.Value;
    }
  }
}
