namespace Faktur.Domain.Receipts;

internal static class ReceiptHelper
{
  public static ReceiptTotal Calculate(IEnumerable<ReceiptItemUnit> items, IReadOnlyDictionary<string, ReceiptTaxUnit> taxes) // TODO(fpion): unit tests
  {
    Dictionary<string, decimal> taxableAmounts = [];
    foreach (KeyValuePair<string, ReceiptTaxUnit> tax in taxes)
    {
      taxableAmounts[tax.Key] = 0.00m;
    }

    decimal subTotal = 0m;
    foreach (ReceiptItemUnit item in items)
    {
      subTotal += item.Price;

      if (item.Flags != null)
      {
        foreach (KeyValuePair<string, ReceiptTaxUnit> tax in taxes)
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
    foreach (KeyValuePair<string, ReceiptTaxUnit> tax in taxes)
    {
      decimal taxableAmount = Math.Round(taxableAmounts[tax.Key], 2);
      decimal amount = Math.Round((decimal)tax.Value.Rate * taxableAmount, 2);
      ReceiptTaxUnit receiptTax = new(tax.Value.Flags, tax.Value.Rate, taxableAmount, amount);
      calculatedTaxes[tax.Key] = receiptTax;
      total += receiptTax.Amount;
    }

    return new ReceiptTotal(Math.Round(subTotal, 2), calculatedTaxes, Math.Round(total, 2));
  }
}
