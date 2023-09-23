using Faktur.Core.Products;

namespace Faktur.Core.Stores
{
  public class Department : Aggregate
  {
    public Department(Store store, Guid userId) : base(userId)
    {
      Store = store ?? throw new ArgumentNullException(nameof(store));
      StoreId = store.Id;
    }
    private Department() : base()
    {
    }

    public string? Description { get; set; }
    public string Name { get; set; } = null!;
    public string? Number { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public Store? Store { get; set; }
    public int StoreId { get; set; }

    public override string ToString() => $"{Name} | {base.ToString()}";
  }
}
