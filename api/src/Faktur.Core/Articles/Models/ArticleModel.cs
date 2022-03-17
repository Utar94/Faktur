using Faktur.Core.Models;

namespace Faktur.Core.Articles.Models
{
  public class ArticleModel : AggregateModel
  {
    public string? Description { get; set; }
    public long? Gtin { get; set; }
    public string Name { get; set; } = null!;
  }
}
