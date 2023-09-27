using Logitar.EventSourcing;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.ValueObjects;
using MediatR;

namespace Logitar.Faktur.Domain.Banners.Events;

public record BannerUpdatedEvent : DomainEvent, INotification
{
  public DisplayNameUnit? DisplayName { get; set; }
  public Modification<DescriptionUnit>? Description { get; set; }

  public bool HasChanges => DisplayName != null || Description != null;
}
