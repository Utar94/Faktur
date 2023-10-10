using Logitar.Faktur.Contracts.Products;
using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.Products;
using Logitar.Faktur.Domain.Stores;

namespace Logitar.Faktur.Application.Products;

public interface IProductQuerier
{
  Task<Product?> ReadAsync(StoreId storeId, ArticleId articleId, CancellationToken cancellationToken = default);
  Task<Product?> ReadAsync(StoreId storeId, SkuUnit sku, CancellationToken cancellationToken = default);
  Task<SearchResults<Product>> SearchAsync(SearchProductsPayload payload, CancellationToken cancellationToken = default);
}
