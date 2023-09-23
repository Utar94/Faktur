using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Articles;

public record ArticleSortOption : SortOption
{
  public new ArticleSort Field { get; set; }

  public ArticleSortOption() : this(ArticleSort.UpdatedOn, isDescending: true)
  {
  }
  public ArticleSortOption(ArticleSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
    Field = field;
  }
}
