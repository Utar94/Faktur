using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Faktur.Domain.Products.Events;

public record ProductSavedEvent : DomainEvent, INotification
{
  public ProductUnit Product { get; init; }

  public ProductSavedEvent(ActorId actorId, ProductUnit product)
  {
    ActorId = actorId;
    Product = product;
  }
}
