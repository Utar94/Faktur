namespace Faktur.Contracts.Products;

public class CreateOrReplaceProductPayload
{
  public string? DepartmentNumber { get; set; }

  public string? Sku { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string? Flags { get; set; }

  public double? UnitPrice { get; set; }
  public UnitType? UnitType { get; set; }
}
