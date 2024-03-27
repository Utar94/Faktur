using Faktur.Domain.Receipts.Events;
using Logitar.EventSourcing;

namespace Faktur.EntityFrameworkCore.Relational.Entities;

internal class ReceiptItemEntity
{
  public ReceiptEntity? Receipt { get; private set; }
  public int ReceiptId { get; private set; }

  public int Number { get; private set; }

  public ProductEntity? Product { get; private set; }
  public int? ProductId { get; private set; }

  public string? Gtin { get; private set; }
  public long? GtinNormalized
  {
    get => Gtin == null ? null : long.Parse(Gtin);
    private set { }
  }
  public string? Sku { get; private set; }
  public string? SkuNormalized
  {
    get => Sku?.ToUpper();
    private set { }
  }

  public string Label { get; private set; } = string.Empty;
  public string? Flags { get; private set; }
  public double Quantity { get; private set; }
  public decimal UnitPrice { get; private set; }
  public decimal Price { get; private set; }

  public string? DepartmentNumber { get; private set; }
  public string? DepartmentName { get; private set; }

  public string CreatedBy { get; private set; } = ActorId.DefaultValue;
  public DateTime CreatedOn { get; private set; }
  public string UpdatedBy { get; private set; } = ActorId.DefaultValue;
  public DateTime UpdatedOn { get; private set; }

  public ReceiptItemEntity(ReceiptEntity receipt, ProductEntity? product, ReceiptItemChangedEvent @event)
  {
    Receipt = receipt;
    ReceiptId = receipt.ReceiptId;

    Number = @event.Number;

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.ToUniversalTime();

    Update(product, @event);
  }

  private ReceiptItemEntity()
  {
  }

  public IEnumerable<ActorId> GetActorIds(bool includeReceipt)
  {
    List<ActorId> actorIds = [new(CreatedBy), new(UpdatedBy)];
    if (includeReceipt && Receipt != null)
    {
      actorIds.AddRange(Receipt.GetActorIds(includeItems: false));
    }
    return actorIds.AsReadOnly();
  }

  public void Update(ProductEntity? product, ReceiptItemChangedEvent @event)
  {
    Product = product;
    ProductId = product?.ProductId;

    Gtin = @event.Item.Gtin?.Value;
    Sku = @event.Item.Sku?.Value;
    Label = @event.Item.Label.Value;
    Flags = @event.Item.Flags?.Value;
    Quantity = @event.Item.Quantity;
    UnitPrice = @event.Item.UnitPrice;
    Price = @event.Item.Price;

    DepartmentNumber = @event.Item.DepartmentNumber?.Value;
    DepartmentName = @event.Item.Department?.DisplayName.Value;

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.ToUniversalTime();
  }
}
