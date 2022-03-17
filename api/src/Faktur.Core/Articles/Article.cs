using Faktur.Core.Products;

namespace Faktur.Core.Articles
{
  public class Article : Aggregate
  {
    public Article(Guid userId) : base(userId)
    {
    }
    private Article() : base()
    {
    }

    public string? Description { get; set; }
    public long? Gtin { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();

    public override string ToString() => $"{Name} | {base.ToString()}";
  }
}
