using Logitar.Validation;
using System.ComponentModel.DataAnnotations;

namespace Faktur.Core.Settings
{
  public class TaxesSettings
  {
    [Required]
    public TaxSettings Gst { get; set; } = null!;

    [Required]
    public TaxSettings Qst { get; set; } = null!;
  }

  public class TaxSettings
  {
    [Required]
    [StringLength(4)]
    public string Code { get; set; } = null!;
    public string CodeNormalized => Code.ToUpperInvariant();

    [Required]
    public string Flag { get; set; } = null!;

    [MinValue(0.00001)]
    public double Rate { get; set; }
  }
}
