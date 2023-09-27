using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Faktur.Domain.Products.Events;

public record ProductRemovedEvent : DomainEvent, INotification
{
  public ProductUnit Product { get; init; }

  public ProductRemovedEvent(ActorId actorId, ProductUnit product)
  {
    ActorId = actorId;
    Product = product;
  }
}
