using Faktur.Contracts.Receipts;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Models.Receipts;

public record SearchReceiptsModel : SearchModel
{
  [FromQuery(Name = "store")]
  public Guid? StoreId { get; set; }

  [FromQuery(Name = "empty")]
  public bool? IsEmpty { get; set; }

  [FromQuery(Name = "processed")]
  public bool? HasBeenProcessed { get; set; }

  public SearchReceiptsPayload ToPayload()
  {
    SearchReceiptsPayload payload = new()
    {
      StoreId = StoreId,
      IsEmpty = IsEmpty,
      HasBeenProcessed = HasBeenProcessed
    };
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new ReceiptSortOption(Enum.Parse<ReceiptSort>(sort)));
      }
      else
      {
        ReceiptSort field = Enum.Parse<ReceiptSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new ReceiptSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
