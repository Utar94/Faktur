namespace Logitar.Faktur.Contracts.Products;

public record SaveProductPayload
{
  public string? DepartmentNumber { get; set; }

  public string? Sku { get; set; }
  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }

  public string? Flags { get; set; }

  public double? UnitPrice { get; set; }
  public string? UnitType { get; set; }
}
