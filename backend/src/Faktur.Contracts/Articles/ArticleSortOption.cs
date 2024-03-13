using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Articles;

public record ArticleSortOption : SortOption
{
  public new ArticleSort Field
  {
    get => Enum.Parse<ArticleSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ArticleSortOption() : this(ArticleSort.UpdatedOn, isDescending: true)
  {
  }

  public ArticleSortOption(ArticleSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
