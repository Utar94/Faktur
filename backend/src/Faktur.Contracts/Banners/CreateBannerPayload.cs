namespace Faktur.Contracts.Banners;

public record CreateBannerPayload
{
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  public CreateBannerPayload() : this(string.Empty)
  {
  }

  public CreateBannerPayload(string displayName)
  {
    DisplayName = displayName;
  }
}
