using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Web.Models;

public record SearchArticlesQuery : SearchQuery
{
  public SearchArticlesPayload ToPayload()
  {
    SearchArticlesPayload payload = new()
    {
      Skip = Skip,
      Limit = Limit
    };
    payload.Id.Terms = IdTerms.Select(term => new SearchTerm(term)).ToList();
    payload.Id.Operator = IdOperator;
    payload.Search.Terms = SearchTerms.Select(term => new SearchTerm(term)).ToList();
    payload.Search.Operator = SearchOperator;

    List<ArticleSortOption> sort = new(capacity: Sort.Count);
    foreach (string sortOption in Sort)
    {
      string[] values = sortOption.Split('.');
      if (values.Length > 2)
      {
        continue;
      }

      if (!Enum.TryParse(values.Length == 2 ? values.Last() : values.Single(), out ArticleSort field))
      {
        continue;
      }

      bool isDescending = values.Length == 2 && values.First().ToLower() == "desc";

      sort.Add(new ArticleSortOption(field, isDescending));
    }
    payload.Sort = sort;

    return payload;
  }
}
