using Faktur.Core.Products;
using Faktur.Core.Settings;
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
    public bool Processed { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public Guid? ProcessedById { get; set; }
    public Store? Store { get; set; }
    public int StoreId { get; set; }
    public decimal SubTotal { get; set; }
    public ICollection<Tax> Taxes { get; set; } = new List<Tax>();
    public decimal Total { get; set; }

    public decimal CalculateSubTotal()
    {
      SubTotal = Math.Round(Items.Sum(x => x.Price), 2);

      return SubTotal;
    }
    public Tax CalculateTax(TaxSettings taxSettings)
    {
      ArgumentNullException.ThrowIfNull(taxSettings);

      Tax? tax = Taxes.SingleOrDefault(x => x.Code == taxSettings.CodeNormalized);
      if (tax == null)
      {
        tax = new Tax(this)
        {
          Code = taxSettings.CodeNormalized,
          Rate = taxSettings.Rate
        };
        Taxes.Add(tax);
      }

      tax.TaxableAmount = 0;
      foreach (Item item in Items)
      {
        Product product = item.Product
          ?? throw new InvalidOperationException($"The {nameof(item.Product)} is required.{Environment.NewLine}Item: {item}");

        if (product.Flags?.Contains(taxSettings.Flag) == true)
        {
          tax.TaxableAmount += item.Price;
        }
      }

      tax.Amount = Math.Round(tax.TaxableAmount * (decimal)tax.Rate, 2);

      return tax;
    }
    public decimal CalculateTotal()
    {
      Total = Math.Round(SubTotal + Taxes.Sum(x => x.Amount), 2);

      return Total;
    }

    public void Process(Guid userId)
    {
      Processed = true;
      ProcessedAt = DateTime.UtcNow;
      ProcessedById = userId;
    }
  }
}
