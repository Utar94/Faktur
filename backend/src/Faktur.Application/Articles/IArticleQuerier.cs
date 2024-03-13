using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using Logitar.Portal.Contracts.Search;

namespace Faktur.Application.Articles;

public interface IArticleQuerier
{
  Task<Article> ReadAsync(ArticleAggregate article, CancellationToken cancellationToken = default);
  Task<Article?> ReadAsync(ArticleId id, CancellationToken cancellationToken = default);
  Task<Article?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Article?> ReadAsync(string gtin, CancellationToken cancellationToken = default);
  Task<SearchResults<Article>> SearchAsync(SearchArticlesPayload payload, CancellationToken cancellationToken = default);
}
