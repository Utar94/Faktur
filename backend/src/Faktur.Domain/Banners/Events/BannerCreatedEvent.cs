using Faktur.Domain.Shared;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Banners.Events;

public record BannerCreatedEvent : DomainEvent, INotification
{
  public DisplayNameUnit DisplayName { get; }

  public BannerCreatedEvent(DisplayNameUnit displayName, ActorId actorId)
  {
    DisplayName = displayName;
    ActorId = actorId;
  }
}
