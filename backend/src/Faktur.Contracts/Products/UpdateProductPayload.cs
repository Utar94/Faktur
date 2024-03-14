namespace Faktur.Contracts.Products;

public class UpdateProductPayload
{
  public Modification<string>? DepartmentNumber { get; set; }

  public Modification<string>? Sku { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public Modification<string>? Flags { get; set; }

  public Modification<decimal?>? UnitPrice { get; set; }
  public Modification<UnitType?>? UnitType { get; set; }
}
