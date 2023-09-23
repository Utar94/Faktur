﻿namespace Logitar.Faktur.Domain.Articles;

public interface IArticleRepository
{
  Task<ArticleAggregate?> LoadAsync(ArticleId id, CancellationToken cancellationToken = default);
  Task<ArticleAggregate?> LoadAsync(ArticleId id, long? version, CancellationToken cancellationToken = default);
  Task SaveAsync(ArticleAggregate article, CancellationToken cancellationToken = default);
}
