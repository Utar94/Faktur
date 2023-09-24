namespace Logitar.Faktur.Contracts.Stores;

public record CreateStorePayload
{
  public string? Id { get; set; }

  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }
}
