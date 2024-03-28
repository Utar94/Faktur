using Faktur.Contracts;
using Faktur.Domain.Banners;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Faktur.Domain.Stores.Events;

public record StoreUpdatedEvent : DomainEvent, INotification
{
  public Modification<BannerId>? BannerId { get; set; }
  public Modification<NumberUnit>? Number { get; set; }
  public DisplayNameUnit? DisplayName { get; set; }
  public Modification<DescriptionUnit>? Description { get; set; }

  public Modification<AddressUnit>? Address { get; set; }
  public Modification<EmailUnit>? Email { get; set; }
  public Modification<PhoneUnit>? Phone { get; set; }

  public bool HasChanges => BannerId != null || Number != null || DisplayName != null || Description != null
    || Address != null || Email != null || Phone != null;
}
