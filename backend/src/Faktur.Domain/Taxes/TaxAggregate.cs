using Faktur.Contracts;
using Faktur.Domain.Products;
using Faktur.Domain.Taxes.Events;
using Logitar.EventSourcing;

namespace Faktur.Domain.Taxes;

public class TaxAggregate : AggregateRoot
{
  private TaxUpdatedEvent _updatedEvent = new();

  public new TaxId Id => new(base.Id);

  private TaxCodeUnit? _code = null;
  public TaxCodeUnit Code
  {
    get => _code ?? throw new InvalidOperationException($"The {nameof(Code)} has not been initialized yet.");
    private set
    {
      if (value != _code)
      {
        _code = value;
        _updatedEvent.Code = value;
      }
    }
  }
  private double _rate = 0;
  public double Rate
  {
    get => _rate;
    private set
    {
      if (value != _rate)
      {
        _rate = value;
        _updatedEvent.Rate = value;
      }
    }
  }

  private FlagsUnit? _flags = null;
  public FlagsUnit? Flags
  {
    get => _flags;
    private set
    {
      if (value != _flags)
      {
        _flags = value;
        _updatedEvent.Flags = new Modification<FlagsUnit>(value);
      }
    }
  }

  public TaxAggregate(AggregateId id) : base(id)
  {
  }

  public TaxAggregate(TaxCodeUnit code, double rate = 0.0, ActorId actorId = default, TaxId? id = null) : base((id ?? TaxId.NewId()).AggregateId)
  {
    Raise(new TaxCreatedEvent(code, rate), actorId);
  }
  protected virtual void Apply(TaxCreatedEvent @event)
  {
    _code = @event.Code;
    _rate = @event.Rate;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new TaxDeletedEvent(), actorId);
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
  protected virtual void Apply(TaxUpdatedEvent @event)
  {
    if (@event.Code != null)
    {
      _code = @event.Code;
    }
    if (@event.Rate.HasValue)
    {
      _rate = @event.Rate.Value;
    }

    if (@event.Flags != null)
    {
      _flags = @event.Flags.Value;
    }
  }

  public override string ToString() => $"{Code.Value} | {base.ToString()}";
}
