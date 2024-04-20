using Faktur.Domain.Receipts;
using Faktur.Domain.Receipts.Events;
using Logitar.EventSourcing;

namespace Faktur.EntityFrameworkCore.Relational.Entities;

internal class ReceiptEntity : AggregateEntity
{
  public int ReceiptId { get; private set; }

  public StoreEntity? Store { get; private set; }
  public int StoreId { get; private set; }

  public DateTime IssuedOn { get; private set; }
  public string? Number { get; private set; }

  public int ItemCount { get; private set; }
  public List<ReceiptItemEntity> Items { get; private set; } = [];

  public decimal SubTotal { get; private set; }
  public decimal Total { get; private set; }
  public List<ReceiptTaxEntity> Taxes { get; private set; } = [];

  public bool HasBeenProcessed
  {
    get => ProcessedBy != null && ProcessedOn != null;
    private set { }
  }
  public string? ProcessedBy { get; private set; }
  public DateTime? ProcessedOn { get; private set; }

  public ReceiptEntity(StoreEntity store, ReceiptCreatedEvent @event) : base(@event)
  {
    Store = store;
    StoreId = store.StoreId;

    IssuedOn = @event.IssuedOn.ToUniversalTime();
    Number = @event.Number?.Value;

    // TODO(fpion): Taxes
  }

  public ReceiptEntity(StoreEntity store, ReceiptImportedEvent @event) : base(@event)
  {
    Store = store;
    StoreId = store.StoreId;

    IssuedOn = @event.IssuedOn.ToUniversalTime();
    Number = @event.Number?.Value;

    Dictionary<string, ProductEntity> products = new(capacity: 2 * store.Products.Count);
    foreach (ProductEntity product in store.Products)
    {
      if (product.Article != null)
      {
        products[$"Gtin:{product.Article.GtinNormalized}"] = product;
      }
      if (product.SkuNormalized != null)
      {
        products[$"Sku:{product.SkuNormalized}"] = product;
      }
    }

    foreach (KeyValuePair<ushort, ReceiptItemUnit> item in @event.Items)
    {
      ProductEntity? product = null;
      if (item.Value.Gtin != null)
      {
        _ = products.TryGetValue($"Gtin:{item.Value.Gtin.NormalizedValue}", out product);
      }
      else if (item.Value.Sku != null)
      {
        _ = products.TryGetValue($"Sku:{item.Value.Sku.Value.ToUpper()}", out product);
      }
      Items.Add(new ReceiptItemEntity(this, product, item.Key, item.Value, @event));
    }
    ItemCount = Items.Count;

    Apply(@event);
  }

  private ReceiptEntity() : base()
  {
  }

  public void Categorize(ReceiptCategorizedEvent @event)
  {
    Update(@event);

    foreach (ReceiptItemEntity item in Items)
    {
      item.Categorize(@event);
    }

    ProcessedBy = @event.ActorId.Value;
    ProcessedOn = @event.OccurredOn.ToUniversalTime();
  }

  public override IEnumerable<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = base.GetActorIds().ToList();
    if (ProcessedBy != null)
    {
      actorIds.Add(new ActorId(ProcessedBy));
    }
    return actorIds.AsReadOnly();
  }
  public IEnumerable<ActorId> GetActorIds(bool includeItems)
  {
    List<ActorId> actorIds = GetActorIds().ToList();
    if (includeItems)
    {
      foreach (ReceiptItemEntity item in Items)
      {
        actorIds.AddRange(item.GetActorIds(includeReceipt: false));
      }
    }
    return actorIds.AsReadOnly();
  }

  public void RemoveItem(ReceiptItemRemovedEvent @event)
  {
    Update(@event);

    ReceiptItemEntity? item = Items.SingleOrDefault(i => i.Number == @event.Number);
    if (item != null)
    {
      Items.Remove(item);
      ItemCount = Items.Count;
    }

    Apply(@event);
  }

  public void SetItem(ProductEntity? product, ReceiptItemChangedEvent @event)
  {
    Update(@event);

    ReceiptItemEntity? item = Items.SingleOrDefault(i => i.Number == @event.Number);
    if (item == null)
    {
      item = new(this, product, @event);
      Items.Add(item);
      ItemCount = Items.Count;
    }
    else
    {
      item.Update(product, @event);
    }

    Apply(@event);
  }

  public void Update(ReceiptUpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.IssuedOn.HasValue)
    {
      IssuedOn = @event.IssuedOn.Value;
    }
    if (@event.Number != null)
    {
      Number = @event.Number.Value?.Value;
    }
  }

  private void Apply(IReceiptTotal total)
  {
    SubTotal = total.SubTotal;
    Total = total.Total;

    foreach (KeyValuePair<string, ReceiptTaxUnit> tax in total.Taxes)
    {
      ReceiptTaxEntity? entity = Taxes.SingleOrDefault(t => t.Code == tax.Key);
      if (entity == null)
      {
        entity = new(this, tax.Key, tax.Value);
        Taxes.Add(entity);
      }
      else
      {
        entity.Update(tax.Value);
      }
    }

    foreach (ReceiptTaxEntity entity in Taxes)
    {
      if (!total.Taxes.ContainsKey(entity.Code))
      {
        Taxes.Remove(entity);
      }
    }
  }
}
