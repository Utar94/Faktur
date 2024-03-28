namespace Faktur.Contracts.Receipts;

public record ReceiptItemCategory
{
  public ushort Number { get; set; }
  public string? Category { get; set; }

  public ReceiptItemCategory() : this(number: 0, category: null)
  {
  }

  public ReceiptItemCategory(ushort number, string? category)
  {
    Number = number;
    Category = category;
  }
}
