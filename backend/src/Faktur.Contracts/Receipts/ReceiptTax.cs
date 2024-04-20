namespace Faktur.Contracts.Receipts;

public record ReceiptTax
{
  public string Code { get; set; }
  public string Flags { get; set; }
  public double Rate { get; set; }
  public decimal TaxableAmount { get; set; }
  public decimal Amount { get; set; }

  public ReceiptTax() : this(string.Empty, string.Empty)
  {
  }

  public ReceiptTax(string code, string flags)
  {
    Code = code;
    Flags = flags;
  }
}
