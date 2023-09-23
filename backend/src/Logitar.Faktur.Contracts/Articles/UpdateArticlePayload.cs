namespace Logitar.Faktur.Contracts.Articles;

public record UpdateArticlePayload
{
  public Modification<string>? Gtin { get; set; }
  public string? DisplayName { get; set; } = string.Empty;
  public Modification<string>? Description { get; set; }
}
