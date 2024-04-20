using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Receipts.Events;

public record ReceiptItemChangedEvent(ushort Number, ReceiptItemUnit Item, decimal SubTotal, IReadOnlyDictionary<string, ReceiptTaxUnit> Taxes, decimal Total)
  : DomainEvent, INotification
{
  public static ReceiptItemChangedEvent Create(ushort number, ReceiptItemUnit item, ReceiptTotal total)
  {
    return new ReceiptItemChangedEvent(number, item, total.SubTotal, total.Taxes, total.Total);
  }
}
