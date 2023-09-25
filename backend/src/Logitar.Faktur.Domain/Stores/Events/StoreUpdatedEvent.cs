using Logitar.EventSourcing;
using Logitar.Faktur.Contracts;
using MediatR;

namespace Logitar.Faktur.Domain.Stores.Events;

public record StoreUpdatedEvent : DomainEvent, INotification
{
  public Modification<AggregateId?>? BannerId { get; set; }

  public Modification<string>? Number { get; set; }
  public string? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public Modification<AddressUnit>? Address { get; set; }
  public Modification<PhoneUnit>? Phone { get; set; }

  public bool HasChanges => Number != null || DisplayName != null || Description != null || Address != null || Phone != null;
}
