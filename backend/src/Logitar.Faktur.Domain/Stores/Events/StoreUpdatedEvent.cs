﻿using Logitar.EventSourcing;
using Logitar.Faktur.Contracts;
using MediatR;

namespace Logitar.Faktur.Domain.Stores.Events;

public record StoreUpdatedEvent : DomainEvent, INotification
{
  public Modification<string>? Number { get; set; }
  public string? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public bool HasChanges => Number != null || DisplayName != null || Description != null;
}
