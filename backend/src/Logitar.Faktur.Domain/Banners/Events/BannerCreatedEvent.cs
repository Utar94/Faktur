using Logitar.EventSourcing;
using Logitar.Faktur.Domain.ValueObjects;
using MediatR;

namespace Logitar.Faktur.Domain.Banners.Events;

public record BannerCreatedEvent : DomainEvent, INotification
{
  public DisplayNameUnit DisplayName { get; init; }

  public BannerCreatedEvent(ActorId actorId, DisplayNameUnit displayName)
  {
    ActorId = actorId;
    DisplayName = displayName;
  }
}
