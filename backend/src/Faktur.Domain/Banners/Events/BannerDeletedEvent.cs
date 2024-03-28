using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Banners.Events;

public record BannerDeletedEvent : DomainEvent, INotification
{
  public BannerDeletedEvent()
  {
    IsDeleted = true;
  }
}
