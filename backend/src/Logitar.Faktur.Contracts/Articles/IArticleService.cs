using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Articles;

public interface IArticleService
{
  Task<AcceptedCommand> CreateAsync(CreateArticlePayload payload, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> DeleteAsync(string id, CancellationToken cancellationToken = default);
  Task<Article?> ReadAsync(string id, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> ReplaceAsync(string id, ReplaceArticlePayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Article>> SearchAsync(SearchArticlesPayload payload, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> UpdateAsync(string id, UpdateArticlePayload payload, CancellationToken cancellationToken = default);
}
