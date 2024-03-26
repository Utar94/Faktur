using Faktur.Contracts.Receipts;

namespace Faktur.Models.Receipts;

public record SearchReceiptItemsModel : SearchModel
{
  public SearchReceiptItemsPayload ToPayload(Guid receiptId)
  {
    SearchReceiptItemsPayload payload = new(receiptId);
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new ReceiptItemSortOption(Enum.Parse<ReceiptItemSort>(sort)));
      }
      else
      {
        ReceiptItemSort field = Enum.Parse<ReceiptItemSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new ReceiptItemSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
