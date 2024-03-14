using Faktur.Contracts.Products;
using Faktur.Domain.Products;
using Logitar.Portal.Contracts.Search;

namespace Faktur.Application.Products;

public interface IProductQuerier
{
  Task<Product> ReadAsync(ProductAggregate product, CancellationToken cancellationToken = default);
  Task<Product?> ReadAsync(ProductId id, CancellationToken cancellationToken = default);
  Task<Product?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Product?> ReadAsync(Guid storeId, Guid articleId, CancellationToken cancellationToken = default);
  Task<Product?> ReadAsync(Guid storeId, string sku, CancellationToken cancellationToken = default);
  Task<SearchResults<Product>> SearchAsync(SearchProductsPayload payload, CancellationToken cancellationToken = default);
}
