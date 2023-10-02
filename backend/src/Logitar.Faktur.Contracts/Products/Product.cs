using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Contracts.Departments;
using Logitar.Faktur.Contracts.Stores;

namespace Logitar.Faktur.Contracts.Products;

public class Product : Metadata
{
  public string? Sku { get; set; }
  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }

  public string? Flags { get; set; }

  public double? UnitPrice { get; set; }
  public string? UnitType { get; set; }

  public Article Article { get; set; } = new();
  public Store Store { get; set; } = new();
  public Department? Department { get; set; }
}
