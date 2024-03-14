using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Products;

public record ProductSortOption : SortOption
{
  public new ProductSort Field
  {
    get => Enum.Parse<ProductSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ProductSortOption() : this(ProductSort.UpdatedOn, isDescending: true)
  {
  }

  public ProductSortOption(ProductSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
