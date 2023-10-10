using Logitar.Faktur.Contracts.Products;
using Logitar.Faktur.Contracts.Search;
using MediatR;

namespace Logitar.Faktur.Application.Products.Queries;

internal class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, SearchResults<Product>>
{
  private readonly IProductQuerier productQuerier;

  public SearchProductsQueryHandler(IProductQuerier productQuerier)
  {
    this.productQuerier = productQuerier;
  }

  public async Task<SearchResults<Product>> Handle(SearchProductsQuery query, CancellationToken cancellationToken)
  {
    return await productQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
