using Faktur.Domain.Stores;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Receipts.Events;

public record ReceiptCreatedEvent : DomainEvent, INotification
{
  public StoreId StoreId { get; }
  public DateTime IssuedOn { get; }
  public NumberUnit? Number { get; }

  public ReceiptCreatedEvent(StoreId storeId, DateTime issuedOn, NumberUnit? number, ActorId actorId)
  {
    StoreId = storeId;
    IssuedOn = issuedOn;
    Number = number;
    ActorId = actorId;
  }
}
