using Faktur.Domain.Receipts;
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

  public string? Category { get; private set; }

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

  public ReceiptItemEntity(ReceiptEntity receipt, ProductEntity? product, ushort number, ReceiptItemUnit item, ReceiptImportedEvent @event)
  {
    Receipt = receipt;
    ReceiptId = receipt.ReceiptId;

    Number = number;

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.ToUniversalTime();

    Update(product, item, @event);
  }

  private ReceiptItemEntity()
  {
  }

  public void Categorize(ReceiptCategorizedEvent @event)
  {
    if (@event.Categories.TryGetValue((ushort)Number, out CategoryUnit? category))
    {
      Category = category?.Value;

      UpdatedBy = @event.ActorId.Value;
      UpdatedOn = @event.OccurredOn.ToUniversalTime();
    }
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
    Update(product, @event.Item, @event);
  }
  private void Update(ProductEntity? product, ReceiptItemUnit item, DomainEvent @event)
  {
    Product = product;
    ProductId = product?.ProductId;

    Gtin = item.Gtin?.Value;
    Sku = item.Sku?.Value;
    Label = item.Label.Value;
    Flags = item.Flags?.Value;
    Quantity = item.Quantity;
    UnitPrice = item.UnitPrice;
    Price = item.Price;

    DepartmentNumber = item.DepartmentNumber?.Value;
    DepartmentName = item.Department?.DisplayName.Value;

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.ToUniversalTime();
  }
}
