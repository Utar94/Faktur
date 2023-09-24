namespace Logitar.Faktur.Contracts.Stores;

public record ReplaceStorePayload
{
  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }
}
