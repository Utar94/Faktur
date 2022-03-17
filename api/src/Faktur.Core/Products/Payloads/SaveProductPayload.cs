using Logitar.Validation;
using System.ComponentModel.DataAnnotations;

namespace Faktur.Core.Products.Payloads
{
  public abstract class SaveProductPayload
  {
    public int? DepartmentId { get; set; }

    public string? Description { get; set; }

    [StringLength(10)]
    public string? Flags { get; set; }

    [StringLength(100)]
    public string? Label { get; set; }

    [StringLength(32)]
    public string? Sku { get; set; }

    [MinValue(0.01)]
    public decimal? UnitPrice { get; set; }

    [StringLength(4)]
    public string? UnitType { get; set; }
  }
}
