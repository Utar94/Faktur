﻿using Logitar.Faktur.Domain.Stores.Events;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Entities;

internal class StoreEntity : AggregateEntity
{
  public int StoreId { get; private set; }

  public string DisplayName { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public StoreEntity(StoreCreatedEvent @event) : base(@event)
  {
    DisplayName = @event.DisplayName;
  }

  private StoreEntity() : base()
  {
  }

  public void Update(StoreUpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value;
    }
  }
}