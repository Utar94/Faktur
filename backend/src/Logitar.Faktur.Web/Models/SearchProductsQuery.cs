using Logitar.Faktur.Contracts.Products;
using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Web.Models;

public record SearchProductsQuery : SearchQuery
{
  public string? DepartmentNumber { get; set; }

  public SearchProductsPayload ToPayload(string storeId)
  {
    SearchProductsPayload payload = new();

    ApplyQuery(payload);

    payload.StoreId = storeId;
    payload.DepartmentNumber = DepartmentNumber;

    List<SortOption> sort = ((SearchPayload)payload).Sort;
    payload.Sort = new List<ProductSortOption>(sort.Capacity);
    foreach (SortOption option in sort)
    {
      if (Enum.TryParse(option.Field, out ProductSort field))
      {
        payload.Sort.Add(new ProductSortOption(field, option.IsDescending));
      }
    }

    return payload;
  }
}
