namespace Logitar.Faktur.Contracts.Articles;

public record ReplaceArticlePayload
{
  public string? Gtin { get; set; }
  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }
}
