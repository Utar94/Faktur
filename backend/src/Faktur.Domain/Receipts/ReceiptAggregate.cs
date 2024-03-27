using Faktur.Contracts;
using Faktur.Domain.Receipts.Events;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;
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

  private readonly Dictionary<ushort, ReceiptItemUnit> _items = [];
  public IReadOnlyDictionary<ushort, ReceiptItemUnit> Items => _items.AsReadOnly();

  public decimal SubTotal { get; private set; }
  private readonly Dictionary<string, ReceiptTaxUnit> _taxes = [];
  public IReadOnlyDictionary<string, ReceiptTaxUnit> Taxes => _taxes.AsReadOnly();
  public decimal Total { get; private set; }

  public ReceiptAggregate(AggregateId id) : base(id)
  {
  }

  public ReceiptAggregate(StoreAggregate store, DateTime? issuedOn = null, NumberUnit? number = null, ActorId actorId = default, ReceiptId? id = null)
    : base((id ?? ReceiptId.NewId()).AggregateId)
  {
    if (issuedOn.HasValue)
    {
      new IssuedOnValidator(nameof(issuedOn)).ValidateAndThrow(issuedOn.Value);
    }

    Raise(new ReceiptCreatedEvent(store.Id, issuedOn ?? DateTime.Now, number, actorId));
  }
  protected virtual void Apply(ReceiptCreatedEvent @event)
  {
    _storeId = @event.StoreId;
    _issuedOn = @event.IssuedOn;
    _number = @event.Number;
  }

  public void Calculate(IEnumerable<TaxAggregate> taxes, ActorId actorId = default)
  {
    Dictionary<string, decimal> taxableAmounts = [];
    foreach (TaxAggregate tax in taxes)
    {
      taxableAmounts[tax.Code.Value] = 0.0m;
    }

    decimal subTotal = 0m;
    foreach (ReceiptItemUnit item in _items.Values)
    {
      subTotal += item.Price;

      if (item.Flags != null)
      {
        foreach (TaxAggregate tax in taxes)
        {
          if (item.IsTaxable(tax))
          {
            taxableAmounts[tax.Code.Value] += item.Price;
          }
        }
      }
    }

    decimal total = subTotal;
    Dictionary<string, ReceiptTaxUnit> receiptTaxes = [];
    foreach (TaxAggregate tax in taxes)
    {
      decimal taxableAmount = taxableAmounts[tax.Code.Value];
      if (taxableAmount > 0)
      {
        ReceiptTaxUnit receiptTax = ReceiptTaxUnit.Calculate(tax.Rate, taxableAmount);
        receiptTaxes[tax.Code.Value] = receiptTax;
        total += receiptTax.Amount;
      }
    }

    Raise(new ReceiptCalculatedEvent(subTotal, receiptTaxes, total), actorId);
  }
  protected virtual void Apply(ReceiptCalculatedEvent @event)
  {
    SubTotal = @event.SubTotal;
    _taxes.Clear();
    foreach (KeyValuePair<string, ReceiptTaxUnit> tax in @event.Taxes)
    {
      _taxes[tax.Key] = tax.Value;
    }
    Total = @event.Total;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new ReceiptDeletedEvent(actorId));
    }
  }

  public bool HasItem(ushort number) => _items.ContainsKey(number);
  public ReceiptItemUnit? TryFindItem(ushort number)
  {
    if (_items.TryGetValue(number, out ReceiptItemUnit? item))
    {
      return item;
    }

    return null;
  }

  public void RemoveItem(ushort number, ActorId actorId = default)
  {
    if (HasItem(number))
    {
      Raise(new ReceiptItemRemovedEvent(number, actorId));
    }
  }
  protected virtual void Apply(ReceiptItemRemovedEvent @event)
  {
    _items.Remove(@event.Number);
  }

  public void SetItem(ushort number, ReceiptItemUnit item, ActorId actorId = default)
  {
    ReceiptItemUnit? existingItem = TryFindItem(number);
    if (existingItem == null || existingItem != item)
    {
      Raise(new ReceiptItemChangedEvent(number, item, actorId));
    }
  }
  protected virtual void Apply(ReceiptItemChangedEvent @event)
  {
    _items[@event.Number] = @event.Item;
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
