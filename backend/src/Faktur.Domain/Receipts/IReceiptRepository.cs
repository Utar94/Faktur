namespace Faktur.Domain.Receipts;

public interface IReceiptRepository
{
  Task SaveAsync(ReceiptAggregate receipt, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ReceiptAggregate> receipts, CancellationToken cancellationToken = default);
}
