using Faktur.Domain.Receipts;
using Faktur.Domain.Receipts.Events;

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
  public List<ReceiptTaxEntity> Taxes { get; private set; } = [];

  public ReceiptEntity(StoreEntity store, ReceiptImportedEvent @event) : base(@event)
  {
    Store = store;
    StoreId = store.StoreId;

    IssuedOn = @event.IssuedOn.ToUniversalTime();
    Number = @event.Number?.Value;

    ItemCount = @event.Items.Count;
    foreach (KeyValuePair<int, ReceiptItemUnit> item in @event.Items)
    {
      ReceiptItemEntity itemEntity = new(this, item.Key, item.Value);
      Items.Add(itemEntity);
    }
  }

  private ReceiptEntity() : base()
  {
  }
}
