using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Taxes;

public record TaxSortOption : SortOption
{
  public new TaxSort Field
  {
    get => Enum.Parse<TaxSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public TaxSortOption() : this(TaxSort.UpdatedOn, isDescending: true)
  {
  }

  public TaxSortOption(TaxSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
