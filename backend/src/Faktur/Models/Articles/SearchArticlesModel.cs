using Faktur.Contracts.Articles;

namespace Faktur.Models.Articles;

public record SearchArticlesModel : SearchModel
{
  public SearchArticlesPayload ToPayload()
  {
    SearchArticlesPayload payload = new();
    Fill(payload);

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      if (index < 0)
      {
        payload.Sort.Add(new ArticleSortOption(Enum.Parse<ArticleSort>(sort)));
      }
      else
      {
        ArticleSort field = Enum.Parse<ArticleSort>(sort[(index + 1)..]);
        bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
        payload.Sort.Add(new ArticleSortOption(field, isDescending));
      }
    }

    return payload;
  }
}
