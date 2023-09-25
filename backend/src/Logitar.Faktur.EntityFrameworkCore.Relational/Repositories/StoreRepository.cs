using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Faktur.Domain.Banners;
using Logitar.Faktur.Domain.Stores;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Repositories;

internal class StoreRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IStoreRepository
{
  private static readonly string AggregateType = typeof(StoreAggregate).GetName();

  private readonly ISqlHelper sqlHelper;

  public StoreRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    this.sqlHelper = sqlHelper;
  }

  public async Task<StoreAggregate?> LoadAsync(StoreId id, CancellationToken cancellationToken)
    => await LoadAsync(id, version: null, cancellationToken);
  public async Task<StoreAggregate?> LoadAsync(StoreId id, long? version, CancellationToken cancellationToken)
    => await LoadAsync<StoreAggregate>(id.AggregateId, version, cancellationToken);

  public async Task<IEnumerable<StoreAggregate>> LoadAsync(BannerAggregate banner, CancellationToken cancellationToken)
  {
    IQuery query = sqlHelper.QueryFrom(Db.Events.Table)
      .Join(Db.Stores.AggregateId, Db.Events.AggregateId,
        new OperatorCondition(Db.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(Db.Banners.BannerId, Db.Stores.BannerId)
      .Where(Db.Banners.AggregateId, Operators.IsEqualTo(banner.Id.Value))
      .SelectAll(Db.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return base.Load<StoreAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task SaveAsync(StoreAggregate store, CancellationToken cancellationToken)
    => await base.SaveAsync(store, cancellationToken);
  public async Task SaveAsync(IEnumerable<StoreAggregate> stores, CancellationToken cancellationToken)
    => await base.SaveAsync(stores, cancellationToken);
}
