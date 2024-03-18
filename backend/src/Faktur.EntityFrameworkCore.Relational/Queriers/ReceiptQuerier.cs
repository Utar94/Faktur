using Faktur.Application.Receipts;
using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Logitar.EventSourcing;

namespace Faktur.EntityFrameworkCore.Relational.Queriers;

internal class ReceiptQuerier : IReceiptQuerier
{
  public async Task<Receipt> ReadAsync(ReceiptAggregate receipt, CancellationToken cancellationToken)
  {
    return await ReadAsync(receipt.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The receipt 'AggregateId={receipt.Id.Value}' could not be found.");
  }
  public async Task<Receipt?> ReadAsync(ReceiptId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public Task<Receipt?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    //StoreEntity? store = await _stores.AsNoTracking()
    //  .Include(x => x.Banner)
    //  .Include(x => x.Departments)
    //  .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    //return store == null ? null : await MapAsync(store, cancellationToken);

    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
