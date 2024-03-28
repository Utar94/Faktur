using Logitar.Portal.Contracts.Actors;

namespace Faktur.Contracts.Receipts;

public class ReceiptItem
{
  public ushort Number { get; set; }

  public string? Gtin { get; set; }
  public string? Sku { get; set; }

  public string Label { get; set; }
  public string? Flags { get; set; }

  public double Quantity { get; set; }
  public decimal UnitPrice { get; set; }
  public decimal Price { get; set; }

  public DepartmentSummary? Department { get; set; }

  public string? Category { get; set; }

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }
  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public Receipt Receipt { get; set; }

  public ReceiptItem() : this(new Receipt(), string.Empty)
  {
  }

  public ReceiptItem(Receipt receipt, string label)
  {
    Receipt = receipt;
    Label = label;
  }

  public override bool Equals(object? obj) => obj is ReceiptItem item && item.Receipt.Id == Receipt.Id && item.Number == Number;
  public override int GetHashCode() => HashCode.Combine(GetType(), Receipt.Id, Number);
  public override string ToString() => $"{Label} | ({GetType()} #{Number}, ReceiptId={Receipt.Id})";
}
