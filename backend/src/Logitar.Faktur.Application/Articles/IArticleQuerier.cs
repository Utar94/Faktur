using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Application.Articles;

public interface IArticleQuerier
{
  Task<Article?> ReadAsync(string id, CancellationToken cancellationToken = default);
  Task<SearchResults<Article>> SearchAsync(SearchArticlesPayload payload, CancellationToken cancellationToken = default);
}
