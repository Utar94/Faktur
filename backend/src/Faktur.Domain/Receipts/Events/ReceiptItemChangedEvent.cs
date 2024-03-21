using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Receipts.Events;

public record ReceiptItemChangedEvent : DomainEvent, INotification
{
  public ushort Number { get; }
  public ReceiptItemUnit Item { get; }

  public ReceiptItemChangedEvent(ushort number, ReceiptItemUnit item, ActorId actorId)
  {
    Number = number;
    Item = item;
    ActorId = actorId;
  }
}
