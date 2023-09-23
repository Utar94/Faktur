using Logitar.Faktur.Contracts.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Faktur.Web.Models;

public record SearchQuery
{
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
}
