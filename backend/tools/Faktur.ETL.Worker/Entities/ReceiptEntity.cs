namespace Faktur.ETL.Worker.Entities;

internal class ReceiptEntity : AggregateEntity
{
  public DateTime IssuedAt { get; set; }
  public string? Number { get; set; }

  public decimal SubTotal { get; set; }
  public decimal Total { get; set; }

  public StoreEntity? Store { get; set; }
  public int StoreId { get; set; }

  public bool Processed { get; set; }
  public DateTime? ProcessedAt { get; set; }
  public Guid? ProcessedById { get; set; }

  public List<ReceiptItemEntity> Items { get; set; } = [];
  public List<ReceiptTaxEntity> Taxes { get; set; } = [];
}
