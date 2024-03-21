using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Receipts;

public record ReceiptItemSortOption : SortOption
{
  public new ReceiptItemSort Field
  {
    get => Enum.Parse<ReceiptItemSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ReceiptItemSortOption() : this(ReceiptItemSort.UpdatedOn, isDescending: true)
  {
  }

  public ReceiptItemSortOption(ReceiptItemSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
