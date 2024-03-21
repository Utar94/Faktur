using Faktur.Contracts.Receipts;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Receipts.Queries;

internal class SearchReceiptItemsQueryHandler : IRequestHandler<SearchReceiptItemsQuery, SearchResults<ReceiptItem>>
{
  private readonly IReceiptItemQuerier _receiptItemQuerier;

  public SearchReceiptItemsQueryHandler(IReceiptItemQuerier receiptItemQuerier)
  {
    _receiptItemQuerier = receiptItemQuerier;
  }

  public async Task<SearchResults<ReceiptItem>> Handle(SearchReceiptItemsQuery query, CancellationToken cancellationToken)
  {
    return await _receiptItemQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
