using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Receipts.Events;

public record ReceiptCategorizedEvent(IReadOnlyDictionary<ushort, CategoryUnit?> Categories) : DomainEvent, INotification;
