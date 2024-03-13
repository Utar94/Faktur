using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Articles.Events;

public record ArticleDeletedEvent : DomainEvent, INotification
{
  public ArticleDeletedEvent(ActorId actorId)
  {
    ActorId = actorId;
    IsDeleted = true;
  }
}
