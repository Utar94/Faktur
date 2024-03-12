namespace Faktur.Contracts.Banners;

public record UpdateBannerPayload
{
  public string? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }
}
