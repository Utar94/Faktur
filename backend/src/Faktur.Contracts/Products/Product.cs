using Faktur.Contracts.Articles;
using Faktur.Contracts.Departments;
using Faktur.Contracts.Stores;
using Logitar.Portal.Contracts;

namespace Faktur.Contracts.Products;

public class Product : Aggregate
{
  public string? Sku { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string? Flags { get; set; }

  public double? UnitPrice { get; set; }
  public UnitType? UnitType { get; set; }

  public Article Article { get; set; }
  public Store Store { get; set; }
  public Department? Department { get; set; }

  public Product() : this(new Article(), new Store())
  {
  }

  public Product(Article article, Store store)
  {
    Article = article;
    Store = store;
  }
}
