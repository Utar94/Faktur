using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Domain.Articles;

namespace Logitar.Faktur.Application.Articles;

public interface IArticleQuerier
{
  Task<Article?> ReadAsync(ArticleId id, CancellationToken cancellationToken = default);
  Task<Article?> ReadAsync(GtinUnit gtin, CancellationToken cancellationToken = default);
  Task<SearchResults<Article>> SearchAsync(SearchArticlesPayload payload, CancellationToken cancellationToken = default);
}
