using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Articles;

public record SearchArticlesPayload : SearchPayload
{
  public new List<ArticleSortOption> Sort { get; set; } = new();
}
