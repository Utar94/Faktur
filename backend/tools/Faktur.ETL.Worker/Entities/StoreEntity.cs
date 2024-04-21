namespace Faktur.ETL.Worker.Entities;

internal class StoreEntity : AggregateEntity
{
  public BannerEntity? Banner { get; set; }
  public int? BannerId { get; set; }

  public string? Number { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }

  public string? Address { get; set; }
  public string? City { get; set; }
  public string? PostalCode { get; set; }
  public string? State { get; set; }
  public string? Country { get; set; }

  public string? Phone { get; set; }

  public List<DepartmentEntity> Departments { get; set; } = [];
  public List<ProductEntity> Products { get; set; } = [];
  public List<ReceiptEntity> Receipts { get; set; } = [];
}
