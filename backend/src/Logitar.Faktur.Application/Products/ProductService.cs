using Logitar.Faktur.Application.Products.Commands;
using Logitar.Faktur.Application.Products.Queries;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Contracts.Products;
using Logitar.Faktur.Contracts.Search;
using MediatR;

namespace Logitar.Faktur.Application.Products;

internal class ProductService : IProductService
{
  private readonly IMediator mediator;

  public ProductService(IMediator mediator)
  {
    this.mediator = mediator;
  }

  public async Task<Product?> ReadAsync(string storeId, string? articleId, string? sku, CancellationToken cancellationToken)
  {
    return await mediator.Send(new ReadProductQuery(storeId, articleId, sku), cancellationToken);
  }

  public async Task<AcceptedCommand> RemoveAsync(string storeId, string articleId, CancellationToken cancellationToken)
  {
    return await mediator.Send(new RemoveProductCommand(storeId, articleId), cancellationToken);
  }

  public async Task<AcceptedCommand> SaveAsync(string storeId, string articleId, SaveProductPayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new SaveProductCommand(storeId, articleId, payload), cancellationToken);
  }

  public async Task<SearchResults<Product>> SearchAsync(SearchProductsPayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new SearchProductsQuery(payload), cancellationToken);
  }

  public async Task<AcceptedCommand> UpdateAsync(string storeId, string articleId, UpdateProductPayload payload, CancellationToken cancellationToken)
  {
    return await mediator.Send(new UpdateProductCommand(storeId, articleId, payload), cancellationToken);
  }
}
