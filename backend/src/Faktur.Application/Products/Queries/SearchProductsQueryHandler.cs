using Faktur.Contracts.Products;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Faktur.Application.Products.Queries;

internal class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, SearchResults<Product>>
{
  private readonly IProductQuerier _productQuerier;

  public SearchProductsQueryHandler(IProductQuerier productQuerier)
  {
    _productQuerier = productQuerier;
  }

  public async Task<SearchResults<Product>> Handle(SearchProductsQuery query, CancellationToken cancellationToken)
  {
    return await _productQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
