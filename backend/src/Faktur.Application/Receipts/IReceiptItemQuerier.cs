using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Logitar.Portal.Contracts.Search;

namespace Faktur.Application.Receipts;

public interface IReceiptItemQuerier
{
  Task<ReceiptItem> ReadAsync(ReceiptAggregate receipt, int number, CancellationToken cancellationToken = default);
  Task<ReceiptItem?> ReadAsync(ReceiptId receiptId, int number, CancellationToken cancellationToken = default);
  Task<ReceiptItem?> ReadAsync(Guid receiptId, int number, CancellationToken cancellationToken = default);
  Task<SearchResults<ReceiptItem>> SearchAsync(SearchReceiptItemsPayload payload, CancellationToken cancellationToken = default);
}
