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
  }

  private ReceiptEntity() : base()
  {
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
  }

  public void SetItem(ReceiptItemChangedEvent @event)
  {
    Update(@event);

    ReceiptItemEntity? item = Items.SingleOrDefault(i => i.Number == @event.Number);
    if (item == null)
    {
      item = new(this, @event);
      Items.Add(item);
      ItemCount = Items.Count;
    }
    else
    {
      item.Update(@event);
    }
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
}
