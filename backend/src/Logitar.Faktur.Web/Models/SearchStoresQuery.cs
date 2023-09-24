using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Contracts.Stores;

namespace Logitar.Faktur.Web.Models;

public record SearchStoresQuery : SearchQuery
{
  public new SearchStoresPayload ToPayload()
  {
    SearchStoresPayload payload = new();

    ApplyQuery(payload);

    List<SortOption> sort = ((SearchPayload)payload).Sort;
    payload.Sort = new List<StoreSortOption>(sort.Capacity);
    foreach (SortOption option in sort)
    {
      if (Enum.TryParse(option.Field, out StoreSort field))
      {
        payload.Sort.Add(new StoreSortOption(field, option.IsDescending));
      }
    }

    return payload;
  }
}
