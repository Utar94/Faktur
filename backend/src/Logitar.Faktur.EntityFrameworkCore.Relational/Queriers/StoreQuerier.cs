using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Faktur.Application.Actors;
using Logitar.Faktur.Application.Stores;
using Logitar.Faktur.Contracts.Actors;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Contracts.Stores;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Queriers;

internal class StoreQuerier : IStoreQuerier
{
  private readonly IActorService actorService;
  private readonly DbSet<StoreEntity> stores;
  private readonly ISqlHelper sqlHelper;

  public StoreQuerier(IActorService actorService, FakturContext context, ISqlHelper sqlHelper)
  {
    this.actorService = actorService;
    stores = context.Stores;
    this.sqlHelper = sqlHelper;
  }

  public async Task<Store?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    id = id.Trim();

    StoreEntity? store = await stores.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

    return store == null ? null : await MapAsync(store, cancellationToken);
  }

  public async Task<SearchResults<Store>> SearchAsync(SearchStoresPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = sqlHelper.QueryFrom(Db.Stores.Table).SelectAll(Db.Stores.Table);
    sqlHelper.ApplyTextSearch(builder, payload.Id, Db.Stores.AggregateId);
    sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Stores.Number, Db.Stores.DisplayName, Db.Stores.AddressFormatted, Db.Stores.PhoneE164Formatted);

    IQueryable<StoreEntity> query = this.stores.FromQuery(builder);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<StoreEntity>? ordered = null;
    foreach (StoreSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case StoreSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.DisplayName) : ordered.OrderBy(x => x.DisplayName));
          break;
        case StoreSort.Number:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number))
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.Number) : ordered.OrderBy(x => x.Number));
          break;
        case StoreSort.PhoneE164Formatted:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.PhoneE164Formatted) : query.OrderBy(x => x.PhoneE164Formatted))
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.PhoneE164Formatted) : ordered.OrderBy(x => x.PhoneE164Formatted));
          break;
        case StoreSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.UpdatedOn) : ordered.OrderBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    StoreEntity[] stores = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Store> results = await MapAsync(stores, cancellationToken);

    return new SearchResults<Store>(results, total);
  }

  private async Task<Store> MapAsync(StoreEntity store, CancellationToken cancellationToken)
    => (await MapAsync(new[] { store }, cancellationToken)).Single();
  private async Task<IEnumerable<Store>> MapAsync(IEnumerable<StoreEntity> stores, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> ids = stores.SelectMany(store => store.GetActorIds());
    IEnumerable<Actor> actors = await actorService.FindAsync(ids, cancellationToken);
    Mapper mapper = new(actors);

    return stores.Select(mapper.ToStore);
  }
}
