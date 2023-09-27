using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Products;

public record ProductSortOption : SortOption
{
  public new ProductSort Field { get; set; }

  public ProductSortOption() : this(ProductSort.UpdatedOn, isDescending: true)
  {
  }
  public ProductSortOption(ProductSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
    Field = field;
  }
}
