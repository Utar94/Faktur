using Faktur.Contracts.Products;
using Logitar.Portal.Contracts;
using MediatR;

namespace Faktur.Application.Products.Queries;

internal class ReadProductQueryHandler : IRequestHandler<ReadProductQuery, Product?>
{
  private readonly IProductQuerier _productQuerier;

  public ReadProductQueryHandler(IProductQuerier productQuerier)
  {
    _productQuerier = productQuerier;
  }

  public async Task<Product?> Handle(ReadProductQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Product> products = new(capacity: 3);

    if (query.Id.HasValue)
    {
      Product? product = await _productQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (product != null)
      {
        products[product.Id] = product;
      }
    }

    if (query.StoreId.HasValue)
    {
      if (query.ArticleId.HasValue)
      {
        Product? product = await _productQuerier.ReadAsync(query.StoreId.Value, query.ArticleId.Value, cancellationToken);
        if (product != null)
        {
          products[product.Id] = product;
        }
      }

      if (!string.IsNullOrWhiteSpace(query.Sku))
      {
        Product? product = await _productQuerier.ReadAsync(query.StoreId.Value, query.Sku, cancellationToken);
        if (product != null)
        {
          products[product.Id] = product;
        }
      }
    }

    if (products.Count > 1)
    {
      throw new TooManyResultsException<Product>(expectedCount: 1, actualCount: products.Count);
    }

    return products.Values.SingleOrDefault();
  }
}
