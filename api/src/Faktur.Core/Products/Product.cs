using Faktur.Core.Articles;
using Faktur.Core.Receipts;
using Faktur.Core.Stores;

namespace Faktur.Core.Products
{
  public class Product : Aggregate
  {
    public Product(Article article, Store store, Guid userId) : base(userId)
    {
      Article = article ?? throw new ArgumentNullException(nameof(article));
      ArticleId = article.Id;
      Store = store ?? throw new ArgumentNullException(nameof(store));
      StoreId = store.Id;
    }
    private Product() : base()
    {
    }

    public Article? Article { get; set; }
    public int ArticleId { get; set; }
    public Department? Department { get; set; }
    public int? DepartmentId { get; set; }
    public string? Description { get; set; }
    public string? Flags { get; set; }
    public ICollection<Item> Items { get; set; } = new List<Item>();
    public string? Label { get; set; }
    public string? Sku { get; set; }
    public Store? Store { get; set; }
    public int StoreId { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? UnitType { get; set; }

    public override string ToString() => string.Join(" | ", new[]
    {
      Label,
      base.ToString()
    }.Where(x => !string.IsNullOrWhiteSpace(x)));
  }
}
