namespace Faktur.Contracts.Receipts;

public record UpdateReceiptItemPayload
{
  public string? GtinOrSku { get; set; }
  public string? Label { get; set; }

  public Modification<string>? Flags { get; set; }

  public double? Quantity { get; set; }
  public decimal? UnitPrice { get; set; }
  public decimal? Price { get; set; }

  public Modification<DepartmentSummary>? Department { get; set; }
}
