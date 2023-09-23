using Faktur.Core.Models;

namespace Faktur.Core.Stores.Models
{
  public class BannerModel : AggregateModel
  {
    public string? Description { get; set; }
    public string Name { get; set; } = null!;
  }
}
