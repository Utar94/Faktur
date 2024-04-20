using Faktur.Domain.Stores;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Receipts.Events;

public record ReceiptImportedEvent(StoreId StoreId, DateTime IssuedOn, NumberUnit? Number, IReadOnlyDictionary<ushort, ReceiptItemUnit> Items,
  decimal SubTotal, IReadOnlyDictionary<string, ReceiptTaxUnit> Taxes, decimal Total) : DomainEvent, INotification, IReceiptTotal
{
  public static ReceiptImportedEvent Create(StoreAggregate store, DateTime? issuedOn, NumberUnit? number, IEnumerable<ReceiptItemUnit>? items, ReceiptTotal total)
  {
    Dictionary<ushort, ReceiptItemUnit> receiptItems = [];
    if (items != null)
    {
      receiptItems = new(capacity: items.Count());
      ushort itemNumber = 0;
      foreach (ReceiptItemUnit item in items)
      {
        receiptItems[itemNumber] = item;
        itemNumber++;
      }
    }

    return new ReceiptImportedEvent(store.Id, issuedOn ?? DateTime.Now, number, receiptItems, total.SubTotal, total.Taxes, total.Total);
  }
}
