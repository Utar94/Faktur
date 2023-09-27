using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.ValueObjects;
using MediatR;

namespace Logitar.Faktur.Domain.Products.Events;

public record ProductSavedEvent : DomainEvent, INotification
{
  public AggregateId ArticleId { get; init; }

  public string DisplayName { get; init; } = string.Empty;
  public string? Description { get; init; }

  public ProductSavedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public static ProductSavedEvent Create(ActorId actorId, ProductUnit product) => new(actorId)
  {
    ArticleId = product.ArticleId.AggregateId,
    DisplayName = product.DisplayName.Value,
    Description = product.Description?.Value
  };

  public ProductUnit GetProduct()
  {
    ArticleId articleId = new(ArticleId);
    DisplayNameUnit displayName = new(DisplayName);
    DescriptionUnit? description = DescriptionUnit.TryCreate(Description);

    return new ProductUnit(articleId, displayName, description);
  }
}
