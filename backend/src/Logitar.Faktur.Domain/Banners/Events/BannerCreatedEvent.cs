using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Faktur.Domain.Banners.Events;

public record BannerCreatedEvent : DomainEvent, INotification
{
  public string DisplayName { get; init; } = string.Empty;

  public BannerCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }
}
