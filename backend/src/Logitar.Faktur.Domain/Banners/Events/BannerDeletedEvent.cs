using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Faktur.Domain.Banners.Events;

public record BannerDeletedEvent : DomainEvent, INotification
{
  public BannerDeletedEvent(ActorId actorId)
  {
    ActorId = actorId;
    IsDeleted = true;
  }
}
