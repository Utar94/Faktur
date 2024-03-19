using Faktur.Contracts.Receipts;
using MediatR;

namespace Faktur.Application.Receipts.Queries;

internal class ReadReceiptQueryHandler : IRequestHandler<ReadReceiptQuery, Receipt?>
{
  private readonly IReceiptQuerier _receiptQuerier;

  public ReadReceiptQueryHandler(IReceiptQuerier receiptQuerier)
  {
    _receiptQuerier = receiptQuerier;
  }

  public async Task<Receipt?> Handle(ReadReceiptQuery query, CancellationToken cancellationToken)
  {
    return await _receiptQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
