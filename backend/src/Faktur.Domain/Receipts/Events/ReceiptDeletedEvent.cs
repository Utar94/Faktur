using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Receipts.Events;

public record ReceiptDeletedEvent : DomainEvent, INotification
{
  public ReceiptDeletedEvent()
  {
    IsDeleted = true;
  }
}
