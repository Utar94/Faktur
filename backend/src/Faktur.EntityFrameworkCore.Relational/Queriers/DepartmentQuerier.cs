using Faktur.Application.Departments;
using Faktur.Contracts.Departments;
using Faktur.Contracts.Stores;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational.Actors;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Queriers;

internal class DepartmentQuerier : IDepartmentQuerier
{
  private readonly IActorService _actorService;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<DepartmentEntity> _departments;

  public DepartmentQuerier(IActorService actorService, FakturContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
    _departments = context.Departments;
  }

  public async Task<Department> ReadAsync(StoreAggregate store, NumberUnit number, CancellationToken cancellationToken)
  {
    return await ReadAsync(store.Id, number.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The department 'StoreId={store.Id.Value},Number={number.Value}' could not be found.");
  }
  public async Task<Department?> ReadAsync(StoreId storeId, string number, CancellationToken cancellationToken)
  {
    return await ReadAsync(storeId.ToGuid(), number, cancellationToken);
  }
  public async Task<Department?> ReadAsync(Guid storeId, string number, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(storeId).Value;
    string numberNormalized = number.Trim().ToUpper();

    DepartmentEntity? department = await _departments.AsNoTracking()
      .Include(x => x.Store)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == aggregateId && x.NumberNormalized == numberNormalized, cancellationToken);

    return department == null ? null : await MapAsync(department, cancellationToken);
  }

  public async Task<SearchResults<Department>> SearchAsync(SearchDepartmentsPayload payload, CancellationToken cancellationToken)
  {
    StoreId storeId = new(payload.StoreId);

    IQueryBuilder builder = _sqlHelper.QueryFrom(FakturDb.Departments.Table).SelectAll(FakturDb.Departments.Table)
      .Join(FakturDb.Stores.StoreId, FakturDb.Departments.StoreId)
      .Where(FakturDb.Stores.AggregateId, Operators.IsEqualTo(storeId.Value));
    _searchHelper.ApplyTextSearch(builder, payload.Search, FakturDb.Departments.Number, FakturDb.Departments.DisplayName);

    IQueryable<DepartmentEntity> query = _departments.FromQuery(builder).AsNoTracking()
      .Include(x => x.Store);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<DepartmentEntity>? ordered = null;
    foreach (DepartmentSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case DepartmentSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case DepartmentSort.Number:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Number) : ordered.ThenBy(x => x.Number));
          break;
        case DepartmentSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    DepartmentEntity[] departments = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Department> items = await MapAsync(departments, cancellationToken);

    return new SearchResults<Department>(items, total);
  }

  private async Task<Department> MapAsync(DepartmentEntity department, CancellationToken cancellationToken)
  {
    return (await MapAsync([department], cancellationToken)).Single();
  }
  private async Task<IEnumerable<Department>> MapAsync(IEnumerable<DepartmentEntity> departments, CancellationToken cancellationToken)
  {
    if (!departments.Any())
    {
      return [];
    }

    Dictionary<int, StoreEntity> stores = new(capacity: departments.Count());
    foreach (DepartmentEntity department in departments)
    {
      if (department.Store != null)
      {
        stores[department.Store.StoreId] = department.Store;
      }
    }
    StoreEntity storeEntity = stores.Values.Single();

    IEnumerable<ActorId> actorIds = storeEntity.GetActorIds(includeDepartments: true);
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Store store = mapper.ToStore(storeEntity, includeDepartments: true);
    return store.Departments;
  }
}
