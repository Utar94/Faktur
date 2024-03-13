using Faktur.Contracts;
using Faktur.Domain.Banners;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores.Events;
using Logitar.EventSourcing;

namespace Faktur.Domain.Stores;

public class StoreAggregate : AggregateRoot
{
  private StoreUpdatedEvent _updatedEvent = new();

  public new StoreId Id => new(base.Id);

  private BannerId? _bannerId = null;
  public BannerId? BannerId
  {
    get => _bannerId;
    set
    {
      if (value != _bannerId)
      {
        _bannerId = value;
        _updatedEvent.BannerId = new Modification<BannerId>(value);
      }
    }
  }
  private NumberUnit? _number = null;
  public NumberUnit? Number
  {
    get => _number;
    set
    {
      if (value != _number)
      {
        _number = value;
        _updatedEvent.Number = new Modification<NumberUnit>(value);
      }
    }
  }
  private DisplayNameUnit? _displayName = null;
  public DisplayNameUnit DisplayName
  {
    get => _displayName ?? throw new InvalidOperationException($"The '{nameof(DisplayName)}' has not been initialized yet.");
    set
    {
      if (value != _displayName)
      {
        _displayName = value;
        _updatedEvent.DisplayName = value;
      }
    }
  }
  private DescriptionUnit? _description = null;
  public DescriptionUnit? Description
  {
    get => _description;
    set
    {
      if (value != _description)
      {
        _description = value;
        _updatedEvent.Description = new Modification<DescriptionUnit>(value);
      }
    }
  }

  public StoreAggregate(AggregateId id) : base(id)
  {
  }

  public StoreAggregate(DisplayNameUnit displayName, ActorId actorId = default, StoreId? id = null)
    : base((id ?? StoreId.NewId()).AggregateId)
  {
    Raise(new StoreCreatedEvent(displayName, actorId));
  }
  protected virtual void Apply(StoreCreatedEvent @event)
  {
    _displayName = @event.DisplayName;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new StoreDeletedEvent(actorId));
    }
  }

  public void Update(ActorId actorId = default)
  {
    if (_updatedEvent.HasChanges)
    {
      _updatedEvent.ActorId = actorId;
      Raise(_updatedEvent);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(StoreUpdatedEvent @event)
  {
    if (@event.BannerId != null)
    {
      _bannerId = @event.BannerId.Value;
    }
    if (@event.Number != null)
    {
      _number = @event.Number.Value;
    }
    if (@event.DisplayName != null)
    {
      _displayName = @event.DisplayName;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }
  }

  public override string ToString() => $"{DisplayName.Value} | {base.ToString()}";
}
