namespace Faktur.ETL.Worker.Entities;

internal class BannerEntity : AggregateEntity
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }

  public List<StoreEntity> Stores { get; set; } = [];
}
