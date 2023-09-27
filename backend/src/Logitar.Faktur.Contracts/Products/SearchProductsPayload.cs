using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Products;

public record SearchProductsPayload : SearchPayload
{
  public string StoreId { get; set; } = string.Empty;
  public string? DepartmentNumber { get; set; }

  public new List<ProductSortOption> Sort { get; set; } = new();
}
