using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Faktur.Domain.Articles.Events;

public record ArticleCreatedEvent : DomainEvent, INotification
{
  public string DisplayName { get; init; } = string.Empty;

  public ArticleCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }
}
