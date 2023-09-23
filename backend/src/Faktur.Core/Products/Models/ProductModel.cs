using Faktur.Core.Articles.Models;
using Faktur.Core.Models;

namespace Faktur.Core.Products.Models
{
  public class ProductModel : AggregateModel
  {
    public ArticleModel? Article { get; set; }
    public int ArticleId { get; set; }
    public int? DepartmentId { get; set; }
    public string? Description { get; set; }
    public string? Flags { get; set; }
    public string? Label { get; set; }
    public string? Sku { get; set; }
    public int StoreId { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? UnitType { get; set; }
  }
}
