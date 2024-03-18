using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;

namespace Faktur.Application.Receipts;

public interface IReceiptQuerier
{
  Task<Receipt> ReadAsync(ReceiptAggregate receipt, CancellationToken cancellationToken = default);
  Task<Receipt?> ReadAsync(ReceiptId id, CancellationToken cancellationToken = default);
  Task<Receipt?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
