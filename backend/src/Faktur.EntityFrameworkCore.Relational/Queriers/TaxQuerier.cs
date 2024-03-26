using Faktur.Application.Taxes;
using Faktur.Contracts.Taxes;
using Faktur.Domain.Taxes;
using Faktur.EntityFrameworkCore.Relational.Actors;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Queriers;

internal class TaxQuerier : ITaxQuerier
{
  private readonly IActorService _actorService;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<TaxEntity> _taxes;

  public TaxQuerier(IActorService actorService, FakturContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
    _taxes = context.Taxes;
  }

  public async Task<Tax> ReadAsync(TaxAggregate tax, CancellationToken cancellationToken)
  {
    return await ReadAsync(tax.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The tax 'AggregateId={tax.Id.Value}' could not be found.");
  }
  public async Task<Tax?> ReadAsync(TaxId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<Tax?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    TaxEntity? tax = await _taxes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return tax == null ? null : await MapAsync(tax, cancellationToken);
  }

  public async Task<Tax?> ReadAsync(string code, CancellationToken cancellationToken)
  {
    string codeNormalized = code.Trim().ToUpper();

    TaxEntity? tax = await _taxes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.CodeNormalized == codeNormalized, cancellationToken);

    return tax == null ? null : await MapAsync(tax, cancellationToken);
  }

  public async Task<SearchResults<Tax>> SearchAsync(SearchTaxesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(FakturDb.Taxes.Table).SelectAll(FakturDb.Taxes.Table)
      .ApplyIdFilter(FakturDb.Taxes.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search, FakturDb.Taxes.Code);

    if (payload.Flag.HasValue)
    {
      builder.Where(FakturDb.Taxes.Flags, _searchHelper.CreateLikeOperator($"%{payload.Flag.Value}%"));
    }

    IQueryable<TaxEntity> query = _taxes.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<TaxEntity>? ordered = null;
    foreach (TaxSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case TaxSort.Code:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Code) : ordered.ThenBy(x => x.Code));
          break;
        case TaxSort.Rate:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Rate) : query.OrderBy(x => x.Rate))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Rate) : ordered.ThenBy(x => x.Rate));
          break;
        case TaxSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    TaxEntity[] taxes = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Tax> items = await MapAsync(taxes, cancellationToken);

    return new SearchResults<Tax>(items, total);
  }

  private async Task<Tax> MapAsync(TaxEntity tax, CancellationToken cancellationToken)
  {
    return (await MapAsync([tax], cancellationToken)).Single();
  }
  private async Task<IEnumerable<Tax>> MapAsync(IEnumerable<TaxEntity> taxes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = taxes.SelectMany(tax => tax.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return taxes.Select(mapper.ToTax);
  }
}
