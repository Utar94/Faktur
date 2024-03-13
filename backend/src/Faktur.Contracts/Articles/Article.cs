using Logitar.Portal.Contracts;

namespace Faktur.Contracts.Articles;

public class Article : Aggregate
{
  public string? Gtin { get; set; }
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  public Article() : this(string.Empty)
  {
  }

  public Article(string displayName)
  {
    DisplayName = displayName;
  }
}
