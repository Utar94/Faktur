using Faktur.Contracts;
using Faktur.Domain.Articles.Events;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Domain.Articles;

public class ArticleAggregate : AggregateRoot
{
  private ArticleUpdatedEvent _updatedEvent = new();

  public new ArticleId Id => new(base.Id);

  private GtinUnit? _gtin = null;
  public GtinUnit? Gtin
  {
    get => _gtin;
    set
    {
      if (value != _gtin)
      {
        _gtin = value;
        _updatedEvent.Gtin = new Modification<GtinUnit>(value);
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

  public ArticleAggregate(AggregateId id) : base(id)
  {
  }

  public ArticleAggregate(DisplayNameUnit displayName, ActorId actorId = default, ArticleId? id = null)
    : base((id ?? ArticleId.NewId()).AggregateId)
  {
    Raise(new ArticleCreatedEvent(displayName), actorId);
  }
  protected virtual void Apply(ArticleCreatedEvent @event)
  {
    _displayName = @event.DisplayName;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new ArticleDeletedEvent(), actorId);
    }
  }

  public void Update(ActorId actorId = default)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, actorId, DateTime.Now);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(ArticleUpdatedEvent @event)
  {
    if (@event.Gtin != null)
    {
      _gtin = @event.Gtin.Value;
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
