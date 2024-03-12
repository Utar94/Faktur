using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Banners;

public record SearchBannersPayload : SearchPayload
{
  public new List<BannerSortOption> Sort { get; set; } = [];
}
