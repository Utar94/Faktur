using Logitar.Faktur.Contracts.Actors;

namespace Logitar.Faktur.Contracts;

public abstract class Aggregate
{
  public string Id { get; set; } = string.Empty;
  public long Version { get; set; }

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }

  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }
}
