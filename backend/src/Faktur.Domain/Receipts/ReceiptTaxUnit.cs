using Faktur.Domain.Products;
using Faktur.Domain.Taxes;
using FluentValidation;
using System.Text.Json.Serialization;

namespace Faktur.Domain.Receipts;

public record ReceiptTaxUnit
{
  public FlagsUnit Flags { get; }
  public double Rate { get; }
  public decimal TaxableAmount { get; }
  public decimal Amount { get; }

  public ReceiptTaxUnit(TaxAggregate tax)
  {
    if (tax.Flags == null)
    {
      throw new ArgumentException($"The {nameof(tax.Flags)} are required.", nameof(tax));
    }

    Flags = tax.Flags;
    Rate = tax.Rate;
    new ReceiptTaxValidator().ValidateAndThrow(this);
  }

  [JsonConstructor]
  public ReceiptTaxUnit(FlagsUnit flags, double rate, decimal taxableAmount, decimal amount)
  {
    Flags = flags;
    Rate = rate;
    TaxableAmount = taxableAmount;
    Amount = amount;
    new ReceiptTaxValidator().ValidateAndThrow(this);
  }
}
