using Faktur.Contracts;
using Faktur.Domain.Shared;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Banners.Events;

public record BannerUpdatedEvent : DomainEvent, INotification
{
  public DisplayNameUnit? DisplayName { get; set; }
  public Modification<DescriptionUnit>? Description { get; set; }

  public bool HasChanges => DisplayName != null || Description != null;
}
