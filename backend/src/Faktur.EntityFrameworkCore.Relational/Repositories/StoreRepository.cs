using Faktur.Domain.Banners;
using Faktur.Domain.Stores;
using Logitar;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Repositories;

internal class StoreRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IStoreRepository
{
  private static readonly string AggregateType = typeof(StoreAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public StoreRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<StoreAggregate?> LoadAsync(StoreId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<StoreAggregate>(id.AggregateId, cancellationToken);
  }
  public async Task<StoreAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<StoreAggregate>(new AggregateId(id), cancellationToken);
  }
  public async Task<StoreAggregate?> LoadAsync(Guid id, long version, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<StoreAggregate>(new AggregateId(id), version, cancellationToken);
  }

  public async Task<IEnumerable<StoreAggregate>> LoadAsync(CancellationToken cancellationToken)
  {
    return await base.LoadAsync<StoreAggregate>(cancellationToken);
  }

  public async Task<IEnumerable<StoreAggregate>> LoadAsync(BannerAggregate banner, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(FakturDb.Stores.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(FakturDb.Banners.BannerId, FakturDb.Stores.BannerId)
      .Where(FakturDb.Banners.AggregateId, Operators.IsEqualTo(banner.Id.Value))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<StoreAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task SaveAsync(StoreAggregate store, CancellationToken cancellationToken)
  {
    await base.SaveAsync(store, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<StoreAggregate> stores, CancellationToken cancellationToken)
  {
    await base.SaveAsync(stores, cancellationToken);
  }
}
