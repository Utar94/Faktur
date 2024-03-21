using Faktur.Contracts.Receipts;
using MediatR;

namespace Faktur.Application.Receipts.Queries;

internal class ReadReceiptItemQueryHandler : IRequestHandler<ReadReceiptItemQuery, ReceiptItem?>
{
  private readonly IReceiptItemQuerier _receiptItemQuerier;

  public ReadReceiptItemQueryHandler(IReceiptItemQuerier receiptItemQuerier)
  {
    _receiptItemQuerier = receiptItemQuerier;
  }

  public async Task<ReceiptItem?> Handle(ReadReceiptItemQuery query, CancellationToken cancellationToken)
  {
    return await _receiptItemQuerier.ReadAsync(query.ReceiptId, query.ItemNumber, cancellationToken);
  }
}
