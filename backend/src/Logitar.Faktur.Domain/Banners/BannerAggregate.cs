using Logitar.EventSourcing;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.Banners.Events;
using Logitar.Faktur.Domain.ValueObjects;

namespace Logitar.Faktur.Domain.Banners;

public class BannerAggregate : AggregateRoot
{
  private BannerUpdatedEvent updated = new();

  public new BannerId Id => new(base.Id);

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

  public BannerAggregate(AggregateId id) : base(id)
  {
  }

  public BannerAggregate(DisplayNameUnit displayName, ActorId actorId = default, BannerId? id = null) : base(id?.AggregateId)
  {
    ApplyChange(new BannerCreatedEvent(actorId)
    {
      DisplayName = displayName.Value
    });
  }
  protected virtual void Apply(BannerCreatedEvent @event)
  {
    displayName = new(@event.DisplayName);
  }

  public void Delete(ActorId actorId = default) => ApplyChange(new BannerDeletedEvent(actorId));

  public void Update(ActorId actorId = default)
  {
    if (updated.HasChanges)
    {
      updated.ActorId = actorId;
      ApplyChange(updated);

      updated = new();
    }
  }
  protected virtual void Apply(BannerUpdatedEvent @event)
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
