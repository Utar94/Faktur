namespace Logitar.Faktur.Contracts.Search;

public record SearchResults<T>
{
  public List<T> Results { get; set; }
  public long Total { get; set; }

  public SearchResults() : this(Enumerable.Empty<T>())
  {
  }
  public SearchResults(IEnumerable<T> results) : this(results, results.LongCount())
  {
  }
  public SearchResults(long total) : this(Enumerable.Empty<T>(), total)
  {
  }
  public SearchResults(IEnumerable<T> results, long total)
  {
    Results = results.ToList();
    Total = total;
  }
}
