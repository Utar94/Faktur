using Faktur.Contracts.Taxes;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Models.Taxes;

public record SearchTaxesModel : SearchModel
{
  [FromQuery(Name = "flag")]
  public char? Flag { get; set; }

  public SearchTaxesPayload ToPayload()
  {
    SearchTaxesPayload payload = new()
    {
      Flag = Flag
    };
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new TaxSortOption(Enum.Parse<TaxSort>(sort)));
      }
      else
      {
        TaxSort field = Enum.Parse<TaxSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new TaxSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
