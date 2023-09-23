using System.ComponentModel.DataAnnotations;

namespace Faktur.Core.Stores.Payloads
{
  public abstract class SaveBannerPayload
  {
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
  }
}
