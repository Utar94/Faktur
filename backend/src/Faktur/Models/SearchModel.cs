using Logitar.Portal.Contracts.Search;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Models;

public record SearchModel
{
  protected const char SortSeparator = '.';
  protected const string IsDescending = "DESC";

  [FromQuery(Name = "ids")]
  public List<Guid> Ids { get; set; } = [];

  [FromQuery(Name = "search_terms")]
  public List<string> SearchTerms { get; set; } = [];

  [FromQuery(Name = "search_operator")]
  public SearchOperator SearchOperator { get; set; }

  [FromQuery(Name = "sort")]
  public List<string> Sort { get; set; } = [];

  [FromQuery(Name = "skip")]
  public int Skip { get; set; }

  [FromQuery(Name = "limit")]
  public int Limit { get; set; }

  protected void Fill(SearchPayload payload, bool fillSort = false)
  {
    payload.Ids = Ids;

    foreach (string term in SearchTerms)
    {
      payload.Search.Terms.Add(new SearchTerm(term));
    }
    payload.Search.Operator = SearchOperator;

    if (fillSort)
    {
      foreach (string sort in Sort)
      {
        int index = sort.IndexOf(SortSeparator);
        if (index < 0)
        {
          payload.Sort.Add(new SortOption(sort));
        }
        else
        {
          string field = sort[(index + 1)..];
          bool isDescending = sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
          payload.Sort.Add(new SortOption(field, isDescending));
        }
      }
    }

    payload.Skip = Skip;
    payload.Limit = Limit;
  }
}
