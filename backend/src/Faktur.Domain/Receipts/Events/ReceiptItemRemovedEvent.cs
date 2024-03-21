using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Receipts.Events;

public record ReceiptItemRemovedEvent : DomainEvent, INotification
{
  public ushort Number { get; }

  public ReceiptItemRemovedEvent(ushort number, ActorId actorId)
  {
    Number = number;
    ActorId = actorId;
  }
}
