using Faktur.Contracts.Taxes;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Taxes.Queries;

internal class SearchTaxesQueryHandler : IRequestHandler<SearchTaxesQuery, SearchResults<Tax>>
{
  private readonly ITaxQuerier _taxQuerier;

  public SearchTaxesQueryHandler(ITaxQuerier taxQuerier)
  {
    _taxQuerier = taxQuerier;
  }

  public async Task<SearchResults<Tax>> Handle(SearchTaxesQuery query, CancellationToken cancellationToken)
  {
    return await _taxQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
