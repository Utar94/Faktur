using System.ComponentModel.DataAnnotations;

namespace Faktur.Core.Stores.Payloads
{
  public abstract class SaveDepartmentPayload
  {
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(32)]
    public string? Number { get; set; }
  }
}
