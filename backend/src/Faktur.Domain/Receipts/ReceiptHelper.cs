using Faktur.Domain.Taxes;

namespace Faktur.Domain.Receipts;

public static class ReceiptHelper
{
  public static ReceiptTotal Calculate(IEnumerable<ReceiptItemUnit> items, IReadOnlyDictionary<TaxCodeUnit, ReceiptTaxUnit> taxes)
  {
    Dictionary<TaxCodeUnit, decimal> taxableAmounts = [];
    foreach (KeyValuePair<TaxCodeUnit, ReceiptTaxUnit> tax in taxes)
    {
      taxableAmounts[tax.Key] = 0.00m;
    }

    decimal subTotal = 0m;
    foreach (ReceiptItemUnit item in items)
    {
      subTotal += item.Price;

      if (item.Flags != null)
      {
        foreach (KeyValuePair<TaxCodeUnit, ReceiptTaxUnit> tax in taxes)
        {
          if (item.IsTaxable(tax.Value))
          {
            taxableAmounts[tax.Key] += item.Price;
          }
        }
      }
    }

    decimal total = subTotal;
    Dictionary<string, ReceiptTaxUnit> calculatedTaxes = [];
    foreach (KeyValuePair<TaxCodeUnit, ReceiptTaxUnit> tax in taxes)
    {
      decimal taxableAmount = Math.Round(taxableAmounts[tax.Key], 2);
      decimal amount = Math.Round((decimal)tax.Value.Rate * taxableAmount, 2);
      ReceiptTaxUnit receiptTax = new(tax.Value.Flags, tax.Value.Rate, taxableAmount, amount);
      calculatedTaxes[tax.Key.Value] = receiptTax;
      total += receiptTax.Amount;
    }

    return new ReceiptTotal(Math.Round(subTotal, 2), calculatedTaxes, Math.Round(total, 2));
  }
}
