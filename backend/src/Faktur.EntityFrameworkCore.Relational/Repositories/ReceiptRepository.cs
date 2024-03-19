using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using Logitar;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Repositories;

internal class ReceiptRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IReceiptRepository
{
  private static readonly string AggregateType = typeof(ReceiptAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public ReceiptRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<ReceiptAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<ReceiptAggregate>(new AggregateId(id), cancellationToken);
  }
  public async Task<ReceiptAggregate?> LoadAsync(Guid id, long version, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<ReceiptAggregate>(new AggregateId(id), version, cancellationToken);
  }

  public async Task<IEnumerable<ReceiptAggregate>> LoadAsync(StoreAggregate store, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(FakturDb.Receipts.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Join(FakturDb.Stores.StoreId, FakturDb.Receipts.StoreId)
      .Where(FakturDb.Stores.AggregateId, Operators.IsEqualTo(store.Id.Value))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<ReceiptAggregate>(events.Select(EventSerializer.Deserialize));
  }

  public async Task SaveAsync(ReceiptAggregate receipt, CancellationToken cancellationToken)
  {
    await base.SaveAsync(receipt, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<ReceiptAggregate> receipts, CancellationToken cancellationToken)
  {
    await base.SaveAsync(receipts, cancellationToken);
  }
}
