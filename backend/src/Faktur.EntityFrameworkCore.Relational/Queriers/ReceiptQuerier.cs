using Faktur.Application.Receipts;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using Faktur.EntityFrameworkCore.Relational.Actors;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Queriers;

internal class ReceiptQuerier : IReceiptQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ReceiptEntity> _receipts;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public ReceiptQuerier(IActorService actorService, FakturContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _receipts = context.Receipts;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<Receipt> ReadAsync(ReceiptAggregate receipt, CancellationToken cancellationToken)
  {
    return await ReadAsync(receipt.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The receipt 'AggregateId={receipt.Id.Value}' could not be found.");
  }
  public async Task<Receipt?> ReadAsync(ReceiptId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<Receipt?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    ReceiptEntity? receipt = await _receipts.AsNoTracking()
      .Include(x => x.Items)
      .Include(x => x.Store)
      .Include(x => x.Taxes)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken); // TODO(fpion): other includes

    return receipt == null ? null : await MapAsync(receipt, cancellationToken);
  }

  public async Task<SearchResults<Receipt>> SearchAsync(SearchReceiptsPayload payload, CancellationToken cancellationToken = default)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(FakturDb.Receipts.Table).SelectAll(FakturDb.Receipts.Table)
      .ApplyIdFilter(FakturDb.Receipts.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search, FakturDb.Receipts.Number);

    if (payload.StoreId.HasValue)
    {
      builder.Join(FakturDb.Stores.StoreId, FakturDb.Receipts.StoreId);

      StoreId storeId = new(payload.StoreId.Value);
      builder.Where(FakturDb.Stores.AggregateId, Operators.IsEqualTo(storeId.Value));
    }
    if (payload.IsEmpty.HasValue)
    {
      ComparisonOperator @operator = payload.IsEmpty.Value ? Operators.IsEqualTo(0) : Operators.IsGreaterThan(0);
      builder.Where(FakturDb.Receipts.ItemCount, @operator);
    }
    if (payload.HasBeenProcessed.HasValue)
    {
      builder.Where(FakturDb.Receipts.HasBeenProcessed, Operators.IsEqualTo(payload.HasBeenProcessed.Value));
    }

    IQueryable<ReceiptEntity> query = _receipts.FromQuery(builder).AsNoTracking()
      .Include(x => x.Store); // TODO(fpion): other includes

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ReceiptEntity>? ordered = null;
    foreach (ReceiptSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ReceiptSort.IssuedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.IssuedOn) : query.OrderBy(x => x.IssuedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.IssuedOn) : ordered.ThenBy(x => x.IssuedOn));
          break;
        case ReceiptSort.Number:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Number) : ordered.ThenBy(x => x.Number));
          break;
        case ReceiptSort.ProcessedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.ProcessedOn) : query.OrderBy(x => x.ProcessedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.ProcessedOn) : ordered.ThenBy(x => x.ProcessedOn));
          break;
        case ReceiptSort.SubTotal:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.SubTotal) : query.OrderBy(x => x.SubTotal))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.SubTotal) : ordered.ThenBy(x => x.SubTotal));
          break;
        case ReceiptSort.Total:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Total) : query.OrderBy(x => x.Total))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Total) : ordered.ThenBy(x => x.Total));
          break;
        case ReceiptSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ReceiptEntity[] stores = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Receipt> items = await MapAsync(stores, cancellationToken);

    return new SearchResults<Receipt>(items, total);
  }

  private async Task<Receipt> MapAsync(ReceiptEntity receipt, CancellationToken cancellationToken)
  {
    return (await MapAsync([receipt], cancellationToken)).Single();
  }
  private async Task<IEnumerable<Receipt>> MapAsync(IEnumerable<ReceiptEntity> receipts, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = receipts.SelectMany(receipt => receipt.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return receipts.Select(mapper.ToReceipt);
  }
}
