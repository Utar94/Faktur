using Faktur.Domain.Taxes;
using Logitar;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Faktur.EntityFrameworkCore.Relational.Repositories;

internal class TaxRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, ITaxRepository
{
  private static readonly string AggregateType = typeof(TaxAggregate).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public TaxRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<TaxAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<TaxAggregate>(new AggregateId(id), cancellationToken);
  }
  public async Task<TaxAggregate?> LoadAsync(Guid id, long version, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<TaxAggregate>(new AggregateId(id), version, cancellationToken);
  }

  public async Task<IEnumerable<TaxAggregate>> LoadAsync(CancellationToken cancellationToken)
  {
    return await base.LoadAsync<TaxAggregate>(cancellationToken);
  }

  public async Task<TaxAggregate?> LoadAsync(TaxCodeUnit code, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(FakturDb.Taxes.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(FakturDb.Taxes.CodeNormalized, Operators.IsEqualTo(code.Value.ToUpper()))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<TaxAggregate>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(TaxAggregate tax, CancellationToken cancellationToken)
  {
    await base.SaveAsync(tax, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<TaxAggregate> taxes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(taxes, cancellationToken);
  }
}
