namespace Faktur.ETL.Worker.Entities;

internal abstract class AggregateEntity
{
  public int Id { get; set; }
  public Guid Key { get; set; }
  public int Version { get; set; }

  public DateTime CreatedAt { get; set; }
  public Guid CreatedById { get; set; }

  public bool Deleted { get; set; }
  public DateTime? DeletedAt { get; set; }
  public Guid? DeletedById { get; set; }

  public DateTime? UpdatedAt { get; set; }
  public Guid? UpdatedById { get; set; }
}
