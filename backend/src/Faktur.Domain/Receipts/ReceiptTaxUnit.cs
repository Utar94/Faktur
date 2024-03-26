namespace Faktur.Domain.Receipts;

public record ReceiptTaxUnit
{
  public double Rate { get; }
  public decimal TaxableAmount { get; }
  public decimal Amount { get; }

  public ReceiptTaxUnit(double rate, decimal taxableAmount, decimal amount)
  {
    Rate = rate;
    TaxableAmount = taxableAmount;
    Amount = amount;
    // TODO(fpion): validate this
  }

  public static ReceiptTaxUnit Calculate(double rate, decimal taxableAmount)
  {
    return new ReceiptTaxUnit(rate, taxableAmount, amount: (decimal)rate * taxableAmount);
  }
}
