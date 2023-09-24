using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Banners;

public record SearchBannersPayload : SearchPayload
{
  public new List<BannerSortOption> Sort { get; set; } = new();
}
