using Faktur.Domain.Stores;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Receipts.Events;

public record ReceiptCreatedEvent(StoreId StoreId, DateTime IssuedOn, NumberUnit? Number) : DomainEvent, INotification;
