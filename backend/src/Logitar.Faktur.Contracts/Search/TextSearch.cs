namespace Logitar.Faktur.Contracts.Search;

public record TextSearch
{
  public List<SearchTerm> Terms { get; set; }
  public SearchOperator Operator { get; set; }

  public TextSearch() : this(Enumerable.Empty<SearchTerm>())
  {
  }
  public TextSearch(IEnumerable<SearchTerm> terms, SearchOperator @operator = SearchOperator.And)
  {
    Terms = terms.ToList();
    Operator = @operator;
  }
}
