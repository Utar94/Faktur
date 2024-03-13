using Faktur.Application.Stores;
using Faktur.Contracts.Stores;
using Faktur.Domain.Banners;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational.Actors;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Queriers;

internal class StoreQuerier : IStoreQuerier
{
  private readonly IActorService _actorService;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<StoreEntity> _stores;

  public StoreQuerier(IActorService actorService, FakturContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
    _stores = context.Stores;
  }

  public async Task<Store> ReadAsync(StoreAggregate store, CancellationToken cancellationToken)
  {
    return await ReadAsync(store.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The store 'AggregateId={store.Id.Value}' could not be found.");
  }
  public async Task<Store?> ReadAsync(StoreId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<Store?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    StoreEntity? store = await _stores.AsNoTracking()
      .Include(x => x.Banner)
      .Include(x => x.Departments)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return store == null ? null : await MapAsync(store, cancellationToken);
  }

  public async Task<SearchResults<Store>> SearchAsync(SearchStoresPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(FakturDb.Stores.Table).SelectAll(FakturDb.Stores.Table)
      .ApplyIdFilter(FakturDb.Stores.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search, FakturDb.Stores.Number, FakturDb.Stores.DisplayName);

    if (payload.BannerId.HasValue)
    {
      builder.Join(FakturDb.Banners.BannerId, FakturDb.Stores.BannerId);

      BannerId bannerId = new(payload.BannerId.Value);
      builder.Where(FakturDb.Banners.AggregateId, Operators.IsEqualTo(bannerId.Value));
    }

    IQueryable<StoreEntity> query = _stores.FromQuery(builder).AsNoTracking()
      .Include(x => x.Banner);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<StoreEntity>? ordered = null;
    foreach (StoreSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case StoreSort.DepartmentCount:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DepartmentCount) : query.OrderBy(x => x.DepartmentCount))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DepartmentCount) : ordered.ThenBy(x => x.DepartmentCount));
          break;
        case StoreSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case StoreSort.Number:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Number) : ordered.ThenBy(x => x.Number));
          break;
        case StoreSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    StoreEntity[] stores = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Store> items = await MapAsync(stores, cancellationToken);

    return new SearchResults<Store>(items, total);
  }

  private async Task<Store> MapAsync(StoreEntity store, CancellationToken cancellationToken)
  {
    return (await MapAsync([store], cancellationToken)).Single();
  }
  private async Task<IEnumerable<Store>> MapAsync(IEnumerable<StoreEntity> stores, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = stores.SelectMany(store => store.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return stores.Select(store => mapper.ToStore(store, includeDepartments: true));
  }
}
