using Faktur.Contracts;
using Faktur.Domain.Stores;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Receipts.Events;

public record ReceiptUpdatedEvent : DomainEvent, INotification
{
  public DateTime? IssuedOn { get; set; }
  public Modification<NumberUnit>? Number { get; set; }

  public bool HasChanges => IssuedOn.HasValue || Number != null;
}
