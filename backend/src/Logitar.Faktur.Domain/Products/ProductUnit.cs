using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.ValueObjects;

namespace Logitar.Faktur.Domain.Products;

public record ProductUnit
{
  public ArticleId ArticleId { get; }
  public DepartmentNumberUnit? DepartmentNumber { get; }

  public SkuUnit? Sku { get; }
  public DisplayNameUnit DisplayName { get; }
  public DescriptionUnit? Description { get; }

  public FlagsUnit? Flags { get; }

  public UnitPriceUnit? UnitPrice { get; }
  public UnitTypeUnit? UnitType { get; }

  public ProductUnit(ArticleId articleId, DisplayNameUnit displayName, DepartmentNumberUnit? departmentNumber = null,
    SkuUnit? sku = null, DescriptionUnit? description = null, FlagsUnit? flags = null, UnitPriceUnit? unitPrice = null, UnitTypeUnit? unitType = null)
  {
    ArticleId = articleId;
    DepartmentNumber = departmentNumber;

    Sku = sku;
    DisplayName = displayName;
    Description = description;

    Flags = flags;

    UnitPrice = unitPrice;
    UnitType = unitType;
  }
}
