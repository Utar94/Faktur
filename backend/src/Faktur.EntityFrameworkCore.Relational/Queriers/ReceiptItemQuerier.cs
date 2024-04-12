using Faktur.Application.Receipts;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.EntityFrameworkCore.Relational.Actors;
using Faktur.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Queriers;

internal class ReceiptItemQuerier : IReceiptItemQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<ReceiptItemEntity> _items;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public ReceiptItemQuerier(IActorService actorService, FakturContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _items = context.ReceiptItems;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<ReceiptItem> ReadAsync(ReceiptAggregate receipt, int number, CancellationToken cancellationToken)
  {
    return await ReadAsync(receipt.Id, number, cancellationToken)
      ?? throw new InvalidOperationException($"The item 'ReceiptId={receipt.Id.Value},Number={number}' could not be found.");
  }
  public async Task<ReceiptItem?> ReadAsync(ReceiptId receiptId, int number, CancellationToken cancellationToken)
  {
    return await ReadAsync(receiptId.ToGuid(), number, cancellationToken);
  }
  public async Task<ReceiptItem?> ReadAsync(Guid receiptId, int number, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(receiptId).Value;

    ReceiptItemEntity? item = await _items.AsNoTracking()
      .Include(x => x.Receipt).ThenInclude(x => x!.Store)
      .Include(x => x.Receipt).ThenInclude(x => x!.Taxes)
      .SingleOrDefaultAsync(x => x.Receipt!.AggregateId == aggregateId && x.Number == number, cancellationToken);

    return item == null ? null : await MapAsync(item, cancellationToken);
  }

  public async Task<SearchResults<ReceiptItem>> SearchAsync(SearchReceiptItemsPayload payload, CancellationToken cancellationToken)
  {
    ReceiptId receiptId = new(payload.ReceiptId);

    IQueryBuilder builder = _sqlHelper.QueryFrom(FakturDb.ReceiptItems.Table).SelectAll(FakturDb.ReceiptItems.Table)
      .Join(FakturDb.Receipts.ReceiptId, FakturDb.ReceiptItems.ReceiptId)
      .Where(FakturDb.Receipts.AggregateId, Operators.IsEqualTo(receiptId.Value));
    _searchHelper.ApplyTextSearch(builder, payload.Search, FakturDb.ReceiptItems.Gtin, FakturDb.ReceiptItems.Sku, FakturDb.ReceiptItems.Label);

    IQueryable<ReceiptItemEntity> query = _items.FromQuery(builder).AsNoTracking()
      .Include(x => x.Receipt).ThenInclude(x => x!.Store)
      .Include(x => x.Receipt).ThenInclude(x => x!.Taxes);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ReceiptItemEntity>? ordered = null;
    foreach (ReceiptItemSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ReceiptItemSort.Gtin:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Gtin) : query.OrderBy(x => x.Gtin))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Gtin) : ordered.ThenBy(x => x.Gtin));
          break;
        case ReceiptItemSort.Label:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Label) : query.OrderBy(x => x.Label))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Label) : ordered.ThenBy(x => x.Label));
          break;
        case ReceiptItemSort.Number:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Number) : ordered.ThenBy(x => x.Number));
          break;
        case ReceiptItemSort.Price:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Price) : ordered.ThenBy(x => x.Price));
          break;
        case ReceiptItemSort.Quantity:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Quantity) : query.OrderBy(x => x.Quantity))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Quantity) : ordered.ThenBy(x => x.Quantity));
          break;
        case ReceiptItemSort.Sku:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Sku) : query.OrderBy(x => x.Sku))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Sku) : ordered.ThenBy(x => x.Sku));
          break;
        case ReceiptItemSort.UnitPrice:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UnitPrice) : query.OrderBy(x => x.UnitPrice))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UnitPrice) : ordered.ThenBy(x => x.UnitPrice));
          break;
        case ReceiptItemSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ReceiptItemEntity[] departments = await query.ToArrayAsync(cancellationToken);
    IEnumerable<ReceiptItem> items = await MapAsync(departments, cancellationToken);

    return new SearchResults<ReceiptItem>(items, total);
  }

  private async Task<ReceiptItem> MapAsync(ReceiptItemEntity item, CancellationToken cancellationToken)
  {
    return (await MapAsync([item], cancellationToken)).Single();
  }
  private async Task<IEnumerable<ReceiptItem>> MapAsync(IEnumerable<ReceiptItemEntity> entities, CancellationToken cancellationToken)
  {
    if (!entities.Any())
    {
      return [];
    }

    IEnumerable<ActorId> actorIds = entities.SelectMany(item => item.GetActorIds(includeReceipt: true));
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Dictionary<int, ReceiptEntity> receipts = new(capacity: entities.Count());
    foreach (ReceiptItemEntity item in entities)
    {
      if (item.Receipt != null)
      {
        receipts[item.Receipt.ReceiptId] = item.Receipt;
      }
    }
    ReceiptEntity receiptEntity = receipts.Values.Single();
    Receipt receipt = mapper.ToReceipt(receiptEntity, includeItems: false);

    IEnumerable<ReceiptItem> items = entities.Select(item => mapper.ToReceiptItem(item, receipt));
    receipt.Items.AddRange(items);
    return receipt.Items;
  }
}
