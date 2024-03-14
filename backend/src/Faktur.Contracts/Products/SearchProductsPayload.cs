using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Products;

public record SearchProductsPayload : SearchPayload
{
  public Guid StoreId { get; set; }
  public string? DepartmentNumber { get; set; }
  public UnitType? UnitType { get; set; }

  public new List<ProductSortOption> Sort { get; set; } = [];

  public SearchProductsPayload() : this(Guid.Empty)
  {
  }

  public SearchProductsPayload(Guid storeId)
  {
    StoreId = storeId;
  }
}
