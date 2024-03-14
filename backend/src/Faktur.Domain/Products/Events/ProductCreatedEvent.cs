using Faktur.Domain.Articles;
using Faktur.Domain.Stores;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Products.Events;

public record ProductCreatedEvent : DomainEvent, INotification
{
  public StoreId StoreId { get; }
  public ArticleId ArticleId { get; }

  public ProductCreatedEvent(StoreId storeId, ArticleId articleId, ActorId actorId)
  {
    StoreId = storeId;
    ArticleId = articleId;
    ActorId = actorId;
  }
}
