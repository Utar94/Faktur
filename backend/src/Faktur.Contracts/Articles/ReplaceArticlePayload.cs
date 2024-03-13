namespace Faktur.Contracts.Articles;

public class ReplaceArticlePayload
{
  public string? Gtin { get; set; }
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  public ReplaceArticlePayload() : this(string.Empty)
  {
  }

  public ReplaceArticlePayload(string displayName)
  {
    DisplayName = displayName;
  }
}
