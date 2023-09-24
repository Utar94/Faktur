using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Banners;

public record BannerSortOption : SortOption
{
  public new BannerSort Field { get; set; }

  public BannerSortOption() : this(BannerSort.UpdatedOn, isDescending: true)
  {
  }
  public BannerSortOption(BannerSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
    Field = field;
  }
}
