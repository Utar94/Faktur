﻿using Logitar.Faktur.Domain.Banners.Events;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Entities;

internal class BannerEntity : AggregateEntity
{
  public int BannerId { get; private set; }

  public string DisplayName { get; private set; } = string.Empty;
  public string? Description { get; private set; }

  public BannerEntity(BannerCreatedEvent @event) : base(@event)
  {
    DisplayName = @event.DisplayName;
  }

  private BannerEntity() : base()
  {
  }

  public void Update(BannerUpdatedEvent @event)
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