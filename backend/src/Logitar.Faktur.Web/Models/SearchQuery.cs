using Logitar.Faktur.Contracts.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Faktur.Web.Models;

public record SearchQuery
{
  public const string DescendingKeyword = "desc";
  public const char SortSeparator = '.';

  [FromQuery(Name = "id_terms")]
  public List<string> IdTerms { get; set; } = new();
  [FromQuery(Name = "id_operator")]
  public SearchOperator IdOperator { get; set; }

  [FromQuery(Name = "search_terms")]
  public List<string> SearchTerms { get; set; } = new();
  [FromQuery(Name = "search_operator")]
  public SearchOperator SearchOperator { get; set; }

  [FromQuery(Name = "sort")]
  public List<string> Sort { get; set; } = new();

  [FromQuery(Name = "skip")]
  public int Skip { get; set; }
  [FromQuery(Name = "limit")]
  public int Limit { get; set; }

  public SearchPayload ToPayload()
  {
    SearchPayload payload = new();

    ApplyQuery(payload);

    return payload;
  }

  protected void ApplyQuery(SearchPayload payload)
  {
    payload.Id.Terms = IdTerms.Select(term => new SearchTerm(term)).ToList();
    payload.Id.Operator = IdOperator;

    payload.Search.Terms = SearchTerms.Select(term => new SearchTerm(term)).ToList();
    payload.Search.Operator = SearchOperator;

    List<SortOption> sort = new(capacity: Sort.Count);
    foreach (string sortOption in Sort)
    {
      string[] values = sortOption.Split(SortSeparator);
      if (values.Length > 2)
      {
        continue;
      }

      string field = values.Length == 2 ? values.Last() : values.Single();
      bool isDescending = values.Length == 2 && values.First().ToLower() == DescendingKeyword;

      sort.Add(new SortOption(field, isDescending));
    }
    payload.Sort = sort;

    payload.Skip = Skip;
    payload.Limit = Limit;
  }
}
