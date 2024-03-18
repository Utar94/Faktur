using Faktur.Contracts;
using Faktur.Domain.Receipts.Events;
using Faktur.Domain.Stores;
using FluentValidation;
using Logitar.EventSourcing;

namespace Faktur.Domain.Receipts;

public class ReceiptAggregate : AggregateRoot
{
  private ReceiptUpdatedEvent _updatedEvent = new();

  public new ReceiptId Id => new(base.Id);

  private StoreId? _storeId = null;
  public StoreId StoreId => _storeId ?? throw new InvalidOperationException($"The '{nameof(StoreId)}' has not been initialized yet.");
  private readonly IssuedOnValidator _issuedOnValidator = new(nameof(IssuedOn));
  private DateTime _issuedOn;
  public DateTime IssuedOn
  {
    get => _issuedOn;
    set
    {
      if (value != _issuedOn)
      {
        _issuedOnValidator.ValidateAndThrow(value);

        _issuedOn = value;
        _updatedEvent.IssuedOn = value;
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

  private readonly Dictionary<int, ReceiptItemUnit> _items = [];
  public IReadOnlyDictionary<int, ReceiptItemUnit> Items => _items.AsReadOnly();

  public ReceiptAggregate(AggregateId id) : base(id)
  {
  }

  public static ReceiptAggregate Import(StoreAggregate store, DateTime? issuedOn = null, NumberUnit? number = null, IEnumerable<ReceiptItemUnit>? items = null, ActorId actorId = default, ReceiptId? id = null)
  {
    if (issuedOn.HasValue)
    {
      new IssuedOnValidator(nameof(issuedOn)).ValidateAndThrow(issuedOn.Value);
    }

    ReceiptAggregate receipt = new((id ?? ReceiptId.NewId()).AggregateId);

    Dictionary<int, ReceiptItemUnit> receiptItems = [];
    if (items != null)
    {
      receiptItems = new(capacity: items.Count());
      int itemNumber = 1;
      foreach (ReceiptItemUnit item in items)
      {
        receiptItems[itemNumber] = item;
        itemNumber++;
      }
    }

    ReceiptImportedEvent @event = new(store.Id, issuedOn ?? DateTime.Now, number, receiptItems, actorId);
    receipt.Raise(@event);

    return receipt;
  }
  protected virtual void Apply(ReceiptImportedEvent @event)
  {
    _storeId = @event.StoreId;
    _issuedOn = @event.IssuedOn;
    _number = @event.Number;

    _items.Clear();
    foreach (KeyValuePair<int, ReceiptItemUnit> item in @event.Items)
    {
      _items[item.Key] = item.Value;
    }
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new ReceiptDeletedEvent(actorId));
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
  protected virtual void Apply(ReceiptUpdatedEvent @event)
  {
    if (@event.IssuedOn.HasValue)
    {
      _issuedOn = @event.IssuedOn.Value;
    }
    if (@event.Number != null)
    {
      _number = @event.Number.Value;
    }
  }
}
