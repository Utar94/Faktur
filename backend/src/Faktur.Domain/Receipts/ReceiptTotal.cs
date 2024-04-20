namespace Faktur.Domain.Receipts;

public record ReceiptTotal(decimal SubTotal, IReadOnlyDictionary<string, ReceiptTaxUnit> Taxes, decimal Total) : IReceiptTotal;
