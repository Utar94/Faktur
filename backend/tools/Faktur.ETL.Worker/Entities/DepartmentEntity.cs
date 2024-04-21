namespace Faktur.ETL.Worker.Entities;

internal class DepartmentEntity : AggregateEntity
{
  public StoreEntity? Store { get; set; }
  public int StoreId { get; set; }

  public string? Number { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }

  public List<ProductEntity> Products { get; set; } = [];
}
