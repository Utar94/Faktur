using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Articles;

public record SearchArticlesPayload : SearchPayload
{
  public new List<ArticleSortOption> Sort { get; set; } = [];
}
