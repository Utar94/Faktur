namespace Logitar.Faktur.Contracts.Articles;

public record CreateArticlePayload
{
  public string? Id { get; set; }

  public string? Gtin { get; set; }
  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }
}
