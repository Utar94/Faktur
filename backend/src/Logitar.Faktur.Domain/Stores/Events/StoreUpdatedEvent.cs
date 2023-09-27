using Logitar.EventSourcing;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.ValueObjects;
using MediatR;

namespace Logitar.Faktur.Domain.Stores.Events;

public record StoreUpdatedEvent : DomainEvent, INotification
{
  public Modification<AggregateId?>? BannerId { get; set; }

  public Modification<StoreNumberUnit>? Number { get; set; }
  public DisplayNameUnit? DisplayName { get; set; }
  public Modification<DescriptionUnit>? Description { get; set; }

  public Modification<AddressUnit>? Address { get; set; }
  public Modification<PhoneUnit>? Phone { get; set; }

  public bool HasChanges => Number != null || DisplayName != null || Description != null || Address != null || Phone != null;
}
