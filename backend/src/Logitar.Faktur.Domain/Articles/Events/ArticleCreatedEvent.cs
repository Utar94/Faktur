using Logitar.EventSourcing;
using Logitar.Faktur.Domain.ValueObjects;
using MediatR;

namespace Logitar.Faktur.Domain.Articles.Events;

public record ArticleCreatedEvent : DomainEvent, INotification
{
  public DisplayNameUnit DisplayName { get; init; }

  public ArticleCreatedEvent(ActorId actorId, DisplayNameUnit displayName)
  {
    ActorId = actorId;
    DisplayName = displayName;
  }
}
