using Faktur.Domain.Stores;

namespace Faktur.Domain.Receipts;

public interface IReceiptRepository
{
  Task<ReceiptAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ReceiptAggregate?> LoadAsync(Guid id, long version, CancellationToken cancellationToken = default);
  Task<IEnumerable<ReceiptAggregate>> LoadAsync(StoreAggregate store, CancellationToken cancellationToken = default);
  Task SaveAsync(ReceiptAggregate receipt, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ReceiptAggregate> receipts, CancellationToken cancellationToken = default);
}
