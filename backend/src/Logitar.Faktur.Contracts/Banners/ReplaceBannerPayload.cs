namespace Logitar.Faktur.Contracts.Banners;

public record ReplaceBannerPayload
{
  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }
}
