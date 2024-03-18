namespace Faktur.Contracts.Receipts;

public record ReceiptTax
{
  public string Code { get; set; }
  public double Rate { get; set; }
  public decimal TaxableAmount { get; set; }
  public decimal Amount { get; set; }

  public ReceiptTax() : this(string.Empty)
  {
  }

  public ReceiptTax(string code)
  {
    Code = code;
  }
}
