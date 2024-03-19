using Faktur.Contracts.Receipts;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Receipts.Queries;

internal class SearchReceiptsQueryHandler : IRequestHandler<SearchReceiptsQuery, SearchResults<Receipt>>
{
  private readonly IReceiptQuerier _receiptQuerier;

  public SearchReceiptsQueryHandler(IReceiptQuerier receiptQuerier)
  {
    _receiptQuerier = receiptQuerier;
  }

  public async Task<SearchResults<Receipt>> Handle(SearchReceiptsQuery query, CancellationToken cancellationToken)
  {
    return await _receiptQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
