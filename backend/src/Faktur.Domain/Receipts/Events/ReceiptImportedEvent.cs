using Faktur.Domain.Stores;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Receipts.Events;

public record ReceiptImportedEvent(StoreId StoreId, DateTime IssuedOn, NumberUnit? Number, IReadOnlyDictionary<ushort, ReceiptItemUnit> Items,
  IReadOnlyDictionary<string, ReceiptTaxUnit> Taxes) : DomainEvent, INotification;
