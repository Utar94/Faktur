using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Stores;

public record StoreSortOption : SortOption
{
  public new StoreSort Field { get; set; }

  public StoreSortOption() : this(StoreSort.UpdatedOn, isDescending: true)
  {
  }
  public StoreSortOption(StoreSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
    Field = field;
  }
}
