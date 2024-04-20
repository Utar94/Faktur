using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Receipts.Events;

public record ReceiptItemRemovedEvent(ushort Number, decimal SubTotal, IReadOnlyDictionary<string, ReceiptTaxUnit> Taxes, decimal Total) : DomainEvent, INotification
{
  public static ReceiptItemRemovedEvent Create(ushort number, ReceiptTotal total)
  {
    return new ReceiptItemRemovedEvent(number, total.SubTotal, total.Taxes, total.Total);
  }
}
