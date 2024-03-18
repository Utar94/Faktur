using Faktur.Domain.Receipts;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;

namespace Faktur.EntityFrameworkCore.Relational.Repositories;

internal class ReceiptRepository : Logitar.EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IReceiptRepository
{
  public ReceiptRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
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
