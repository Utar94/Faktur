namespace Faktur.ETL.Worker.Entities;

internal class ProductEntity : AggregateEntity
{
  public string? Sku { get; set; }
  public string? Label { get; set; }
  public string? Description { get; set; }

  public string? Flags { get; set; }

  public decimal? UnitPrice { get; set; }
  public string? UnitType { get; set; }

  public ArticleEntity? Article { get; set; }
  public int ArticleId { get; set; }
  public StoreEntity? Store { get; set; }
  public int StoreId { get; set; }
  public DepartmentEntity? Department { get; set; }
  public int? DepartmentId { get; set; }

  public List<ReceiptItemEntity> ReceiptItems { get; set; } = [];
}
