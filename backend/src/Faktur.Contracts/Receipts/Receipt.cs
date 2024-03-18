using Faktur.Contracts.Stores;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;

namespace Faktur.Contracts.Receipts;

public class Receipt : Aggregate
{
  public DateTime? IssuedOn { get; set; }
  public string? Number { get; set; }

  public List<ReceiptItem> Items { get; set; }

  public decimal SubTotal { get; set; }
  public List<ReceiptTax> Taxes { get; set; }
  public decimal Total { get; set; }

  public bool HasBeenProcessed { get; set; }
  public Actor? ProcessedBy { get; set; }
  public DateTime? ProcessedOn { get; set; }

  public Store Store { get; set; }

  public Receipt() : this(new Store())
  {
  }

  public Receipt(Store store)
  {
    Store = store;
    Items = [];
    Taxes = [];
  }
}
