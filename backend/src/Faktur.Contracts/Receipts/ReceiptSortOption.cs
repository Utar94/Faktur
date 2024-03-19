using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Receipts;

public record ReceiptSortOption : SortOption
{
  public new ReceiptSort Field
  {
    get => Enum.Parse<ReceiptSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ReceiptSortOption() : this(ReceiptSort.UpdatedOn, isDescending: true)
  {
  }

  public ReceiptSortOption(ReceiptSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
