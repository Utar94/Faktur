using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Web.Models;

public record SearchArticlesQuery : SearchQuery
{
  public new SearchArticlesPayload ToPayload()
  {
    SearchArticlesPayload payload = new();

    ApplyQuery(payload);

    List<SortOption> sort = ((SearchPayload)payload).Sort;
    payload.Sort = new List<ArticleSortOption>(sort.Capacity);
    foreach (SortOption option in sort)
    {
      if (Enum.TryParse(option.Field, out ArticleSort field))
      {
        payload.Sort.Add(new ArticleSortOption(field, option.IsDescending));
      }
    }

    return payload;
  }
}
