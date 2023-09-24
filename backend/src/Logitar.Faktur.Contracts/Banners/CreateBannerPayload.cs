namespace Logitar.Faktur.Contracts.Banners;

public record CreateBannerPayload
{
  public string? Id { get; set; }

  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }
}
