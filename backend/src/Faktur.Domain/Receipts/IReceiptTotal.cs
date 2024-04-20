namespace Faktur.Domain.Receipts;

public interface IReceiptTotal
{
  decimal SubTotal { get; }
  IReadOnlyDictionary<string, ReceiptTaxUnit> Taxes { get; }
  decimal Total { get; }
}
