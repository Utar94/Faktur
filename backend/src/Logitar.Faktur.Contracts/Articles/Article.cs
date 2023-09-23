namespace Logitar.Faktur.Contracts.Articles;

public class Article : Aggregate
{
  public string? Gtin { get; set; }
  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }
}
