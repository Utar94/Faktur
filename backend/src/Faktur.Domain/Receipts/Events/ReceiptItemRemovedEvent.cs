using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Receipts.Events;

public record ReceiptItemRemovedEvent(ushort Number) : DomainEvent, INotification;
