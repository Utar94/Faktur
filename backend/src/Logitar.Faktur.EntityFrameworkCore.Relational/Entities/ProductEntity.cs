using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Products.Events;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Entities;

internal class ProductEntity : Entity
{
  public int ProductId { get; private set; }

  public StoreEntity? Store { get; private set; }
  public int StoreId { get; private set; }

  public DepartmentEntity? Department { get; private set; }
  public int? DepartmentId { get; private set; }

  public ArticleEntity? Article { get; private set; }
  public int ArticleId { get; private set; }

  public string? Sku { get; private set; }
  public string? SkuNormalized
  {
    get => Sku?.ToUpper();
    private set { }
  }
  public string DisplayName { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public string? Flags { get; private set; }

  public double? UnitPrice { get; private set; }
  public string? UnitType { get; private set; }

  public long Version { get; private set; }

  public string CreatedBy { get; private set; } = string.Empty;
  public DateTime CreatedOn { get; private set; }

  public string UpdatedBy { get; private set; } = string.Empty;
  public DateTime UpdatedOn { get; private set; }

  public ProductEntity(ProductSavedEvent @event, StoreEntity store, DepartmentEntity? department, ArticleEntity article)
  {
    Store = store;
    StoreId = store.StoreId;

    Article = article;
    ArticleId = article.ArticleId;

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.ToUniversalTime();

    Update(@event, department);
  }

  private ProductEntity() : base()
  {
  }

  public IEnumerable<ActorId> GetActorIds() => GetActorIds(addStore: true);
  public IEnumerable<ActorId> GetActorIds(bool addStore)
  {
    List<ActorId> ids = new(capacity: 6)
    {
      new ActorId(CreatedBy),
      new ActorId(UpdatedBy)
    };

    if (addStore && Store != null)
    {
      ids.AddRange(Store.GetActorIds(addDepartments: false, addProducts: false));
    }

    return ids;
  }

  public void Update(ProductSavedEvent @event, DepartmentEntity? department)
  {
    Department = department;
    DepartmentId = department?.DepartmentId;

    Sku = @event.Product.Sku?.Value;
    DisplayName = @event.Product.DisplayName.Value;
    Description = @event.Product.Description?.Value;

    Flags = @event.Product.Flags?.Value;

    UnitPrice = @event.Product.UnitPrice?.Value;
    UnitType = @event.Product.UnitType?.Value;

    Version++;

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.ToUniversalTime();
  }
}
