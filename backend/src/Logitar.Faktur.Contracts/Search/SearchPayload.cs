namespace Logitar.Faktur.Contracts.Search;

public record SearchPayload
{
  public TextSearch Id { get; set; } = new();
  public TextSearch Search { get; set; } = new();

  public List<SortOption> Sort { get; set; } = new();

  public int Skip { get; set; }
  public int Limit { get; set; }
}
