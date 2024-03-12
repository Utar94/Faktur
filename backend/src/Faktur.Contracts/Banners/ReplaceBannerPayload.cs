namespace Faktur.Contracts.Banners;

public record ReplaceBannerPayload
{
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  public ReplaceBannerPayload() : this(string.Empty)
  {
  }

  public ReplaceBannerPayload(string displayName)
  {
    DisplayName = displayName;
  }
}
