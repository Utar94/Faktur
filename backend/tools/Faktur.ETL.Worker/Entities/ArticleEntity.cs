namespace Faktur.ETL.Worker.Entities;

internal class ArticleEntity : AggregateEntity
{
  public long? Gtin { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }

  public List<ProductEntity> Products { get; set; } = [];
}
