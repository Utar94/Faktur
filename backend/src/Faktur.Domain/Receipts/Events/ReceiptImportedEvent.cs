using Faktur.Domain.Stores;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Receipts.Events;

public record ReceiptImportedEvent : DomainEvent, INotification
{
  public StoreId StoreId { get; }
  public DateTime IssuedOn { get; }
  public NumberUnit? Number { get; }
  public Dictionary<int, ReceiptItemUnit> Items { get; }

  public ReceiptImportedEvent(StoreId storeId, DateTime issuedOn, NumberUnit? number, Dictionary<int, ReceiptItemUnit> items, ActorId actorId)
  {
    StoreId = storeId;
    IssuedOn = issuedOn;
    Number = number;
    ActorId = actorId;
    Items = items;
  }
}
