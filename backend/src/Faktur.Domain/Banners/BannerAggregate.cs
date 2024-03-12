using Faktur.Contracts;
using Faktur.Domain.Banners.Events;
using Faktur.Domain.Shared;
using Logitar.EventSourcing;

namespace Faktur.Domain.Banners;

public class BannerAggregate : AggregateRoot
{
  private BannerUpdatedEvent _updatedEvent = new();

  public new BannerId Id => new(base.Id);

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

  public BannerAggregate(AggregateId id) : base(id)
  {
  }

  public BannerAggregate(DisplayNameUnit displayName, ActorId actorId = default, BannerId? id = null)
    : base((id ?? BannerId.NewId()).AggregateId)
  {
    Raise(new BannerCreatedEvent(displayName, actorId));
  }
  protected virtual void Apply(BannerCreatedEvent @event)
  {
    _displayName = @event.DisplayName;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new BannerDeletedEvent(actorId));
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
  protected virtual void Apply(BannerUpdatedEvent @event)
  {
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
