using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Articles;
using MediatR;

namespace Logitar.Faktur.Domain.Products.Events;

public record ProductRemovedEvent : DomainEvent, INotification
{
  public AggregateId ArticleId { get; init; }

  public ProductRemovedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public static ProductRemovedEvent Create(ActorId actorId, ArticleId articleId) => new(actorId)
  {
    ArticleId = articleId.AggregateId
  };

  public ArticleId GetArticleId() => new(ArticleId);
}
