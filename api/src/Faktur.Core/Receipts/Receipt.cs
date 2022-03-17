using Faktur.Core.Stores;

namespace Faktur.Core.Receipts
{
  public class Receipt : Aggregate
  {
    public Receipt(Store store, Guid userId) : base(userId)
    {
      Store = store ?? throw new ArgumentNullException(nameof(store));
      StoreId = store.Id;
    }
    private Receipt() : base()
    {
    }

    public DateTime IssuedAt { get; set; }
    public ICollection<Item> Items { get; set; } = new List<Item>();
    public string? Number { get; set; }
    public Store? Store { get; set; }
    public int StoreId { get; set; }
    public decimal SubTotal
    {
      get => Items.Sum(x => x.Price);
      set
      {
      }
    }
    public ICollection<Tax> Taxes { get; set; } = new List<Tax>();
    public decimal Total
    {
      get => SubTotal + Taxes.Sum(x => x.Amount);
      set
      {
      }
    }
  }
}
