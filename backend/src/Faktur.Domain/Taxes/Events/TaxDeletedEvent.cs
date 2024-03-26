using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Taxes.Events;

public record TaxDeletedEvent : DomainEvent, INotification
{
  public TaxDeletedEvent()
  {
    IsDeleted = true;
  }
}
