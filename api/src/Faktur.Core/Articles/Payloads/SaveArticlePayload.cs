using System.ComponentModel.DataAnnotations;

namespace Faktur.Core.Articles.Payloads
{
  public abstract class SaveArticlePayload
  {
    public string? Description { get; set; }

    [Range(0, 99999999999999)]
    public long? Gtin { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
  }
}
