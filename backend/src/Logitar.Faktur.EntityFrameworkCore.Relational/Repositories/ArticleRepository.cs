using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Faktur.Domain.Articles;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Repositories;

internal class ArticleRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IArticleRepository
{
  private static readonly string AggregateType = typeof(ArticleAggregate).GetName();

  private readonly ISqlHelper sqlHelper;

  public ArticleRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    this.sqlHelper = sqlHelper;
  }

  public async Task<ArticleAggregate?> LoadAsync(ArticleId id, CancellationToken cancellationToken)
    => await LoadAsync(id, version: null, cancellationToken);
  public async Task<ArticleAggregate?> LoadAsync(ArticleId id, long? version, CancellationToken cancellationToken)
    => await LoadAsync<ArticleAggregate>(id.AggregateId, version, cancellationToken);

  public async Task<ArticleAggregate?> LoadAsync(GtinUnit gtin, CancellationToken cancellationToken)
  {
    IQuery query = sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Articles.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(Db.Articles.GtinNormalized, Operators.IsEqualTo(gtin.NormalizedValue))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<ArticleAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(ArticleAggregate article, CancellationToken cancellationToken)
    => await base.SaveAsync(article, cancellationToken);
  public async Task SaveAsync(IEnumerable<ArticleAggregate> articles, CancellationToken cancellationToken)
    => await base.SaveAsync(articles, cancellationToken);
}
