using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Contracts.Products;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.Products;
using Logitar.Faktur.Domain.Stores;
using MediatR;

namespace Logitar.Faktur.Application.Products.Queries;

internal class ReadProductQueryHandler : IRequestHandler<ReadProductQuery, Product?>
{
  private readonly IProductQuerier productQuerier;

  public ReadProductQueryHandler(IProductQuerier productQuerier)
  {
    this.productQuerier = productQuerier;
  }

  public async Task<Product?> Handle(ReadProductQuery query, CancellationToken cancellationToken)
  {
    Dictionary<string, Product> products = new(capacity: 2);

    StoreId storeId = StoreId.Parse(query.StoreId, nameof(query.StoreId));

    if (!string.IsNullOrWhiteSpace(query.ArticleId))
    {
      ArticleId articleId = ArticleId.Parse(query.ArticleId, nameof(query.ArticleId));
      Product? product = await productQuerier.ReadAsync(storeId, articleId, cancellationToken);
      if (product != null)
      {
        products[product.Article.Id] = product;
      }
    }

    SkuUnit? sku = SkuUnit.TryCreate(query.Sku);
    if (sku != null)
    {
      Product? product = await productQuerier.ReadAsync(storeId, sku, cancellationToken);
      if (product != null)
      {
        products[product.Article.Id] = product;
      }
    }

    if (products.Count > 1)
    {
      throw new TooManyResultsException<Product>(expected: 1, actual: products.Count);
    }

    return products.Values.SingleOrDefault();
  }
}
