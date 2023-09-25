using FluentValidation;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Faktur.Application.Actors;
using Logitar.Faktur.Application.Departments;
using Logitar.Faktur.Contracts.Actors;
using Logitar.Faktur.Contracts.Departments;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Queriers;

internal class DepartmentQuerier : IDepartmentQuerier
{
  private readonly IActorService actorService;
  private readonly DbSet<DepartmentEntity> departments;
  private readonly ISqlHelper sqlHelper;

  public DepartmentQuerier(IActorService actorService, FakturContext context, ISqlHelper sqlHelper)
  {
    this.actorService = actorService;
    departments = context.Departments;
    this.sqlHelper = sqlHelper;
  }

  public async Task<Department?> ReadAsync(string storeId, string number, CancellationToken cancellationToken)
  {
    new DepartmentNumberValidator(nameof(number)).ValidateAndThrow(number);

    storeId = storeId.Trim();
    int numberNormalized = DepartmentNumberUnit.Parse(number, nameof(number)).NormalizedValue;

    DepartmentEntity? department = await departments.AsNoTracking()
      .Include(x => x.Store)
      .SingleOrDefaultAsync(x => x.Store!.AggregateId == storeId && x.NumberNormalized == numberNormalized, cancellationToken);

    return department == null ? null : await MapAsync(department, cancellationToken);
  }

  public async Task<SearchResults<Department>> SearchAsync(SearchDepartmentsPayload payload, CancellationToken cancellationToken)
  {
    string storeId = payload.StoreId.Trim();

    IQueryBuilder builder = sqlHelper.QueryFrom(Db.Departments.Table)
      .Join(Db.Stores.StoreId, Db.Departments.StoreId)
      .Where(Db.Stores.AggregateId, Operators.IsEqualTo(storeId))
      .SelectAll(Db.Departments.Table);
    sqlHelper.ApplyTextSearch(builder, payload.Id, Db.Departments.Number);
    sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Departments.Number, Db.Departments.DisplayName);

    IQueryable<DepartmentEntity> query = this.departments.FromQuery(builder).AsNoTracking()
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
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.DisplayName) : ordered.OrderBy(x => x.DisplayName));
          break;
        case DepartmentSort.Number:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.NumberNormalized) : query.OrderBy(x => x.NumberNormalized))
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.NumberNormalized) : ordered.OrderBy(x => x.NumberNormalized));
          break;
        case DepartmentSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.OrderByDescending(x => x.UpdatedOn) : ordered.OrderBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    DepartmentEntity[] departments = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Department> results = await MapAsync(departments, cancellationToken);

    return new SearchResults<Department>(results, total);
  }

  private async Task<Department> MapAsync(DepartmentEntity department, CancellationToken cancellationToken)
    => (await MapAsync(new[] { department }, cancellationToken)).Single();
  private async Task<IEnumerable<Department>> MapAsync(IEnumerable<DepartmentEntity> departments, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> ids = departments.SelectMany(department => department.GetActorIds());
    IEnumerable<Actor> actors = await actorService.FindAsync(ids, cancellationToken);
    Mapper mapper = new(actors);

    return departments.Select(mapper.ToDepartment);
  }
}
