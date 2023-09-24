namespace Logitar.Faktur.Contracts.Stores;

public record UpdateStorePayload
{
  public string? DisplayName { get; set; } = string.Empty;
  public Modification<string>? Description { get; set; }
}
