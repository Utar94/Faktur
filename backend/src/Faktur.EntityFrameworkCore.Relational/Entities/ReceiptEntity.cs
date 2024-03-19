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
    List<ActorId> actorIds = new(capacity: 3);
    actorIds.AddRange(base.GetActorIds());
    if (ProcessedBy != null)
    {
      actorIds.Add(new ActorId(ProcessedBy));
    }
    return actorIds;
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
