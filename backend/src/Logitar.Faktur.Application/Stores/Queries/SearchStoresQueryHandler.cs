using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Contracts.Stores;
using MediatR;

namespace Logitar.Faktur.Application.Stores.Queries;

internal class SearchStoresQueryHandler : IRequestHandler<SearchStoresQuery, SearchResults<Store>>
{
  private readonly IStoreQuerier storeQuerier;

  public SearchStoresQueryHandler(IStoreQuerier storeQuerier)
  {
    this.storeQuerier = storeQuerier;
  }

  public async Task<SearchResults<Store>> Handle(SearchStoresQuery query, CancellationToken cancellationToken)
  {
    return await storeQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
