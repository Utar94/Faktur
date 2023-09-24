﻿using Logitar.EventSourcing;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.Articles.Events;
using Logitar.Faktur.Domain.ValueObjects;

namespace Logitar.Faktur.Domain.Articles;

public class ArticleAggregate : AggregateRoot
{
  private ArticleUpdatedEvent updated = new();

  public new ArticleId Id => new(base.Id);

  private GtinUnit? gtin = null;
  public GtinUnit? Gtin
  {
    get => gtin;
    set
    {
      if (value != gtin)
      {
        updated.Gtin = new Modification<string>(value?.Value);
        gtin = value;
      }
    }
  }
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

  public ArticleAggregate(AggregateId id) : base(id)
  {
  }

  public ArticleAggregate(DisplayNameUnit displayName, ActorId actorId = default, ArticleId? id = null) : base(id?.AggregateId)
  {
    ApplyChange(new ArticleCreatedEvent(actorId)
    {
      DisplayName = displayName.Value
    });
  }
  protected virtual void Apply(ArticleCreatedEvent @event)
  {
    displayName = new(@event.DisplayName);
  }

  public void Delete(ActorId actorId = default) => ApplyChange(new ArticleDeletedEvent(actorId));

  public void Update(ActorId actorId = default)
  {
    if (updated.HasChanges)
    {
      updated.ActorId = actorId;
      ApplyChange(updated);

      updated = new();
    }
  }
  protected virtual void Apply(ArticleUpdatedEvent @event)
  {
    if (@event.Gtin != null)
    {
      gtin = @event.Gtin.Value == null ? null : new GtinUnit(@event.Gtin.Value);
    }
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