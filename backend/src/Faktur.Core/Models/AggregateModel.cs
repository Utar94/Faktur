namespace Faktur.Core.Models
{
  public abstract class AggregateModel
  {
    public DateTime CreatedAt { get; set; }
    public bool Deleted { get; set; }
    public int Id { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int Version { get; set; }
  }
}
