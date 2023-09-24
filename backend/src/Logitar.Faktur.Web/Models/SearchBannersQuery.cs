using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Web.Models;

public record SearchBannersQuery : SearchQuery
{
  public new SearchBannersPayload ToPayload()
  {
    SearchBannersPayload payload = new();

    ApplyQuery(payload);

    List<SortOption> sort = ((SearchPayload)payload).Sort;
    payload.Sort = new List<BannerSortOption>(sort.Capacity);
    foreach (SortOption option in sort)
    {
      if (Enum.TryParse(option.Field, out BannerSort field))
      {
        payload.Sort.Add(new BannerSortOption(field, option.IsDescending));
      }
    }

    return payload;
  }
}
