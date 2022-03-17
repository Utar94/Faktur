using System.ComponentModel.DataAnnotations;

namespace Faktur.Core.Stores.Payloads
{
  public abstract class SaveStorePayload
  {
    public int? BannerId { get; set; }

    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(32)]
    public string? Number { get; set; }



    [StringLength(100)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(2, MinimumLength = 2)]
    public string? Country { get; set; }

    [StringLength(40)]
    public string? Phone { get; set; }

    [RegularExpression("^[ABCEGHJ-NPRSTVXY]\\d[ABCEGHJ-NPRSTV-Z][ -]?\\d[ABCEGHJ-NPRSTV-Z]\\d$")]
    public string? PostalCode { get; set; }

    [StringLength(2, MinimumLength = 2)]
    public string? State { get; set; }
  }
}
