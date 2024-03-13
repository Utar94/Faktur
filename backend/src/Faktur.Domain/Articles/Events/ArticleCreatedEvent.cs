using Faktur.Domain.Shared;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Articles.Events;

public record ArticleCreatedEvent : DomainEvent, INotification
{
  public DisplayNameUnit DisplayName { get; }

  public ArticleCreatedEvent(DisplayNameUnit displayName, ActorId actorId)
  {
    DisplayName = displayName;
    ActorId = actorId;
  }
}
