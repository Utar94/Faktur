namespace Faktur.Contracts.Articles;

public class UpdateArticlePayload
{
  public Modification<string>? Gtin { get; set; }
  public string? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }
}
