using Faktur.Domain.Articles;
using Logitar;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Repositories;

internal class ArticleRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IArticleRepository
{
  private static readonly string AggregateType = typeof(ArticleAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public ArticleRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<ArticleAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<ArticleAggregate>(new AggregateId(id), cancellationToken);
  }
  public async Task<ArticleAggregate?> LoadAsync(Guid id, long version, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<ArticleAggregate>(new AggregateId(id), version, cancellationToken);
  }
  public async Task<ArticleAggregate?> LoadAsync(GtinUnit gtin, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(FakturDb.Articles.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(FakturDb.Articles.GtinNormalized, Operators.IsEqualTo(gtin.NormalizedValue))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<ArticleAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task<IEnumerable<ArticleAggregate>> LoadAsync(CancellationToken cancellationToken)
  {
    return await base.LoadAsync<ArticleAggregate>(cancellationToken);
  }

  public async Task SaveAsync(ArticleAggregate article, CancellationToken cancellationToken)
  {
    await base.SaveAsync(article, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<ArticleAggregate> articles, CancellationToken cancellationToken)
  {
    await base.SaveAsync(articles, cancellationToken);
  }
}
