namespace Faktur.Contracts.Receipts;

public record CreateOrReplaceReceiptItemPayload
{
  public string GtinOrSku { get; set; }
  public string Label { get; set; }

  public string? Flags { get; set; }

  public double? Quantity { get; set; }
  public decimal? UnitPrice { get; set; }
  public decimal Price { get; set; }

  public DepartmentPayload? Department { get; set; }

  public CreateOrReplaceReceiptItemPayload() : this(string.Empty, string.Empty)
  {
  }

  public CreateOrReplaceReceiptItemPayload(string gtinOrSku, string label)
  {
    GtinOrSku = gtinOrSku;
    Label = label;
  }
}
