﻿using Faktur.Contracts;
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

  private readonly Dictionary<ushort, CategoryUnit> _categories = [];
  public IReadOnlyDictionary<ushort, CategoryUnit> Categories => _categories.AsReadOnly();

  public decimal SubTotal { get; private set; }
  private readonly Dictionary<TaxCodeUnit, ReceiptTaxUnit> _taxes = [];
  public IReadOnlyDictionary<TaxCodeUnit, ReceiptTaxUnit> Taxes => _taxes.AsReadOnly();
  public decimal Total { get; private set; }

  public ReceiptAggregate(AggregateId id) : base(id)
  {
  }

  public ReceiptAggregate(StoreAggregate store, DateTime? issuedOn = null, NumberUnit? number = null, IEnumerable<TaxAggregate>? taxes = null,
    ActorId actorId = default, ReceiptId? id = null) : base((id ?? ReceiptId.NewId()).AggregateId)
  {
    if (issuedOn.HasValue)
    {
      new IssuedOnValidator(nameof(issuedOn)).ValidateAndThrow(issuedOn.Value);
    }

    Dictionary<string, ReceiptTaxUnit> receiptTaxes = [];
    if (taxes != null)
    {
      receiptTaxes = new(capacity: taxes.Count());
      foreach (TaxAggregate tax in taxes)
      {
        if (tax.Flags != null)
        {
          receiptTaxes[tax.Code.Value] = new ReceiptTaxUnit(tax.Flags, tax.Rate, taxableAmount: 0.00m, amount: 0.00m);
        }
      }
    }

    Raise(new ReceiptCreatedEvent(store.Id, issuedOn ?? DateTime.Now, number, receiptTaxes), actorId);
  }
  protected virtual void Apply(ReceiptCreatedEvent @event)
  {
    _storeId = @event.StoreId;
    _issuedOn = @event.IssuedOn;
    _number = @event.Number;

    _taxes.Clear();
    foreach (KeyValuePair<string, ReceiptTaxUnit> tax in @event.Taxes)
    {
      TaxCodeUnit key = new(tax.Key);
      _taxes[key] = tax.Value;
    }
  }

  public static ReceiptAggregate Import(StoreAggregate store, DateTime? issuedOn = null, NumberUnit? number = null, IEnumerable<ReceiptItemUnit>? items = null,
    IEnumerable<TaxAggregate>? taxes = null, ActorId actorId = default, ReceiptId? id = null)
  {
    if (issuedOn.HasValue)
    {
      new IssuedOnValidator(nameof(issuedOn)).ValidateAndThrow(issuedOn.Value);
    }

    ReceiptAggregate receipt = new((id ?? ReceiptId.NewId()).AggregateId);

    Dictionary<TaxCodeUnit, ReceiptTaxUnit> receiptTaxes = [];
    if (taxes != null)
    {
      receiptTaxes = new(capacity: taxes.Count());
      foreach (TaxAggregate tax in taxes)
      {
        if (tax.Flags != null)
        {
          receiptTaxes[tax.Code] = new ReceiptTaxUnit(tax);
        }
      }
    }

    ReceiptTotal total = ReceiptHelper.Calculate(items ?? [], receiptTaxes);

    receipt.Raise(ReceiptImportedEvent.Create(store, issuedOn, number, items, total), actorId);

    return receipt;
  }
  protected virtual void Apply(ReceiptImportedEvent @event)
  {
    _storeId = @event.StoreId;
    _issuedOn = @event.IssuedOn;
    _number = @event.Number;

    _items.Clear();
    foreach (KeyValuePair<ushort, ReceiptItemUnit> item in @event.Items)
    {
      _items[item.Key] = item.Value;
    }

    Apply((IReceiptTotal)@event);
  }

  public void Categorize(IEnumerable<KeyValuePair<ushort, CategoryUnit?>> itemCategories, ActorId actorId = default)
  {
    Dictionary<ushort, CategoryUnit?> updatedCategories = new(capacity: itemCategories.Count());
    foreach (KeyValuePair<ushort, CategoryUnit?> itemCategory in itemCategories)
    {
      if (_items.ContainsKey(itemCategory.Key))
      {
        CategoryUnit? existingCategory = TryFindCategory(itemCategory.Key);
        if (existingCategory != itemCategory.Value)
        {
          updatedCategories[itemCategory.Key] = itemCategory.Value;
        }
      }
    }

    if (updatedCategories.Count > 0)
    {
      Raise(new ReceiptCategorizedEvent(updatedCategories), actorId);
    }
  }
  protected virtual void Apply(ReceiptCategorizedEvent @event)
  {
    foreach (KeyValuePair<ushort, CategoryUnit?> itemCategory in @event.Categories)
    {
      if (itemCategory.Value == null)
      {
        _categories.Remove(itemCategory.Key);
      }
      else
      {
        _categories[itemCategory.Key] = itemCategory.Value;
      }
    }
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new ReceiptDeletedEvent(), actorId);
    }
  }

  public bool HasCategory(ushort number) => _categories.ContainsKey(number);
  public CategoryUnit? TryFindCategory(ushort number)
  {
    if (_categories.TryGetValue(number, out CategoryUnit? category))
    {
      return category;
    }

    return null;
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
      Dictionary<ushort, ReceiptItemUnit> items = new(_items);
      items.Remove(number);

      ReceiptTotal total = ReceiptHelper.Calculate(items.Values, _taxes);
      Raise(ReceiptItemRemovedEvent.Create(number, total), actorId);
    }
  }
  protected virtual void Apply(ReceiptItemRemovedEvent @event)
  {
    _categories.Remove(@event.Number);
    _items.Remove(@event.Number);

    Apply((IReceiptTotal)@event);
  }

  public void SetItem(ushort number, ReceiptItemUnit item, ActorId actorId = default)
  {
    ReceiptItemUnit? existingItem = TryFindItem(number);
    if (existingItem == null || existingItem != item)
    {
      Dictionary<ushort, ReceiptItemUnit> items = new(_items)
      {
        [number] = item
      };

      ReceiptTotal total = ReceiptHelper.Calculate(items.Values, _taxes);
      Raise(ReceiptItemChangedEvent.Create(number, item, total), actorId);
    }
  }
  protected virtual void Apply(ReceiptItemChangedEvent @event)
  {
    _items[@event.Number] = @event.Item;

    Apply((IReceiptTotal)@event);
  }

  public void Update(ActorId actorId = default)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, actorId, DateTime.Now);
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

  private void Apply(IReceiptTotal total)
  {
    SubTotal = total.SubTotal;

    _taxes.Clear();
    foreach (KeyValuePair<string, ReceiptTaxUnit> tax in total.Taxes)
    {
      TaxCodeUnit key = new(tax.Key);
      _taxes[key] = tax.Value;
    }

    Total = total.Total;
  }
}
