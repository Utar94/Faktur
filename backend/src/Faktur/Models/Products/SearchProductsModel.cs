using Faktur.Contracts.Products;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Models.Products;

public record SearchProductsModel : SearchModel
{
  [FromQuery(Name = "department")]
  public string? DepartmentNumber { get; set; }

  [FromQuery(Name = "unit_type")]
  public UnitType? UnitType { get; set; }

  public SearchProductsPayload ToPayload(Guid storeId)
  {
    SearchProductsPayload payload = new(storeId)
    {
      DepartmentNumber = DepartmentNumber,
      UnitType = UnitType
    };
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new ProductSortOption(Enum.Parse<ProductSort>(sort)));
      }
      else
      {
        ProductSort field = Enum.Parse<ProductSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new ProductSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
