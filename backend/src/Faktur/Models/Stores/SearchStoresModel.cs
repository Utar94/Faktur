using Faktur.Contracts.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Models.Stores;

public record SearchStoresModel : SearchModel
{
  [FromQuery(Name = "banner")]
  public Guid? BannerId { get; set; }

  public SearchStoresPayload ToPayload()
  {
    SearchStoresPayload payload = new()
    {
      BannerId = BannerId
    };
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new StoreSortOption(Enum.Parse<StoreSort>(sort)));
      }
      else
      {
        StoreSort field = Enum.Parse<StoreSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new StoreSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
