using Faktur.Core.Models;

namespace Faktur.Core.Receipts.Models
{
  public class ReceiptModel : AggregateModel
  {
    public DateTime IssuedAt { get; set; }
    public IEnumerable<ItemModel> Items { get; set; } = Enumerable.Empty<ItemModel>();
    public string? Number { get; set; }
    public int StoreId { get; set; }
    public decimal SubTotal { get; set; }
    public IEnumerable<TaxModel> Taxes { get; set; } = Enumerable.Empty<TaxModel>();
    public decimal Total { get; set; }
  }
}
