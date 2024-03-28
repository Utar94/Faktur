using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Receipts.Events;

public record ReceiptItemChangedEvent(ushort Number, ReceiptItemUnit Item) : DomainEvent, INotification;
