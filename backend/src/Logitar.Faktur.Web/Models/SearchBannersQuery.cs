using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Web.Models;

public record SearchBannersQuery : SearchQuery
{
  public SearchBannersPayload ToPayload() // TODO(fpion): refactor
  {
    SearchBannersPayload payload = new()
    {
      Skip = Skip,
      Limit = Limit
    };
    payload.Id.Terms = IdTerms.Select(term => new SearchTerm(term)).ToList();
    payload.Id.Operator = IdOperator;
    payload.Search.Terms = SearchTerms.Select(term => new SearchTerm(term)).ToList();
    payload.Search.Operator = SearchOperator;

    List<BannerSortOption> sort = new(capacity: Sort.Count);
    foreach (string sortOption in Sort)
    {
      string[] values = sortOption.Split('.');
      if (values.Length > 2)
      {
        continue;
      }

      if (!Enum.TryParse(values.Length == 2 ? values.Last() : values.Single(), out BannerSort field))
      {
        continue;
      }

      bool isDescending = values.Length == 2 && values.First().ToLower() == "desc";

      sort.Add(new BannerSortOption(field, isDescending));
    }
    payload.Sort = sort;

    return payload;
  }
}
