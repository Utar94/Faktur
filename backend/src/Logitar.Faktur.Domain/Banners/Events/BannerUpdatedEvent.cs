using Logitar.EventSourcing;
using Logitar.Faktur.Contracts;
using MediatR;

namespace Logitar.Faktur.Domain.Banners.Events;

public record BannerUpdatedEvent : DomainEvent, INotification
{
  public string? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public bool HasChanges => DisplayName != null || Description != null;
}
