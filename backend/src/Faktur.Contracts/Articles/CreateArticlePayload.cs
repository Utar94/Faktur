namespace Faktur.Contracts.Articles;

public class CreateArticlePayload
{
  public string? Gtin { get; set; }
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  public CreateArticlePayload() : this(string.Empty)
  {
  }

  public CreateArticlePayload(string displayName)
  {
    DisplayName = displayName;
  }
}
