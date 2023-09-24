namespace Logitar.Faktur.Contracts.Banners;

public record UpdateBannerPayload
{
  public string? DisplayName { get; set; } = string.Empty;
  public Modification<string>? Description { get; set; }
}
