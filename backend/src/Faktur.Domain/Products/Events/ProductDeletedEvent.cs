using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Products.Events;

public record ProductDeletedEvent : DomainEvent, INotification
{
  public ProductDeletedEvent()
  {
    IsDeleted = true;
  }
}
