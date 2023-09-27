namespace Logitar.Faktur.Contracts.Products;

public record UpdateProductPayload
{
  public Modification<string>? DepartmentNumber { get; set; }

  public Modification<string>? Sku { get; set; }
  public string? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public Modification<string>? Flags { get; set; }

  public Modification<double>? UnitPrice { get; set; }
  public Modification<string>? UnitType { get; set; }
}
