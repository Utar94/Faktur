using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Products;

public interface IProductService
{
  Task<Product?> ReadAsync(string storeId, string? articleId = null, string? sku = null, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> RemoveAsync(string storeId, string articleId, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> SaveAsync(string storeId, string articleId, SaveProductPayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<Product>> SearchAsync(SearchProductsPayload payload, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> UpdateAsync(string storeId, string articleId, UpdateProductPayload payload, CancellationToken cancellationToken = default);
}
