using Faktur.Contracts.Banners;

namespace Faktur.Models.Banners;

public record SearchBannersModel : SearchModel
{
  public SearchBannersPayload ToPayload()
  {
    SearchBannersPayload payload = new();
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new BannerSortOption(Enum.Parse<BannerSort>(sort)));
      }
      else
      {
        BannerSort field = Enum.Parse<BannerSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new BannerSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
