using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Banners;

public record BannerSortOption : SortOption
{
  public new BannerSort Field
  {
    get => Enum.Parse<BannerSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public BannerSortOption() : this(BannerSort.UpdatedOn, isDescending: true)
  {
  }

  public BannerSortOption(BannerSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
