using Faktur.Contracts;
using Faktur.Domain.Banners;
using Faktur.Domain.Shared;
using Logitar.EventSourcing;
using MediatR;

namespace Faktur.Domain.Stores.Events;

public record StoreUpdatedEvent : DomainEvent, INotification
{
  public Modification<BannerId>? BannerId { get; set; }
  public Modification<NumberUnit>? Number { get; set; }
  public DisplayNameUnit? DisplayName { get; set; }
  public Modification<DescriptionUnit>? Description { get; set; }

  public bool HasChanges => BannerId != null || Number != null || DisplayName != null || Description != null;
}
