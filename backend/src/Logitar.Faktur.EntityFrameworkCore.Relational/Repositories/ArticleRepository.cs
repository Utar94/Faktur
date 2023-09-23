using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Faktur.Domain.Articles;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Repositories;

internal class ArticleRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IArticleRepository
{
  public ArticleRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task<ArticleAggregate?> LoadAsync(ArticleId id, CancellationToken cancellationToken)
    => await LoadAsync(id, version: null, cancellationToken);
  public async Task<ArticleAggregate?> LoadAsync(ArticleId id, long? version, CancellationToken cancellationToken)
    => await LoadAsync<ArticleAggregate>(id.AggregateId, version, cancellationToken);

  public async Task SaveAsync(ArticleAggregate article, CancellationToken cancellationToken)
    => await base.SaveAsync(article, cancellationToken);
}
