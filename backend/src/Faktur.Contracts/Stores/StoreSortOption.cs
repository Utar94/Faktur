using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Stores;

public record StoreSortOption : SortOption
{
  public new StoreSort Field
  {
    get => Enum.Parse<StoreSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public StoreSortOption() : this(StoreSort.UpdatedOn, isDescending: true)
  {
  }

  public StoreSortOption(StoreSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
