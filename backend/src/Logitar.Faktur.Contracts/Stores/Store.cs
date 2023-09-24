namespace Logitar.Faktur.Contracts.Stores;

public class Store : Aggregate
{
  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }
}
