﻿using Logitar.EventSourcing;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.Stores.Events;
using Logitar.Faktur.Domain.ValueObjects;

namespace Logitar.Faktur.Domain.Stores;

public class StoreAggregate : AggregateRoot
{
  private StoreUpdatedEvent updated = new();

  public new StoreId Id => new(base.Id);

  private DisplayNameUnit? displayName = null;
  public DisplayNameUnit DisplayName
  {
    get => displayName ?? throw new InvalidOperationException($"The {nameof(DisplayName)} has not been initialized.");
    set
    {
      if (value != displayName)
      {
        updated.DisplayName = value.Value;
        displayName = value;
      }
    }
  }
  private DescriptionUnit? description = null;
  public DescriptionUnit? Description
  {
    get => description;
    set
    {
      if (value != description)
      {
        updated.Description = new Modification<string>(value?.Value);
        description = value;
      }
    }
  }

  public StoreAggregate(AggregateId id) : base(id)
  {
  }

  public StoreAggregate(DisplayNameUnit displayName, ActorId actorId = default, StoreId? id = null) : base(id?.AggregateId)
  {
    ApplyChange(new StoreCreatedEvent(actorId)
    {
      DisplayName = displayName.Value
    });
  }
  protected virtual void Apply(StoreCreatedEvent @event)
  {
    displayName = new(@event.DisplayName);
  }

  public void Delete(ActorId actorId = default) => ApplyChange(new StoreDeletedEvent(actorId));

  public void Update(ActorId actorId = default)
  {
    if (updated.HasChanges)
    {
      updated.ActorId = actorId;
      ApplyChange(updated);

      updated = new();
    }
  }
  protected virtual void Apply(StoreUpdatedEvent @event)
  {
    if (@event.DisplayName != null)
    {
      displayName = @event.DisplayName == null ? null : new DisplayNameUnit(@event.DisplayName);
    }
    if (@event.Description != null)
    {
      description = @event.Description.Value == null ? null : new DescriptionUnit(@event.Description.Value);
    }
  }

  public override string ToString() => $"{DisplayName.Value} | {base.ToString()}";
}