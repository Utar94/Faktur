using Faktur.Domain.Articles;
using Faktur.Domain.Stores;

namespace Faktur.Domain.Products;

public interface IProductRepository
{
  Task<ProductAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ProductAggregate?> LoadAsync(ProductId id, long version, CancellationToken cancellationToken = default);
  Task<ProductAggregate?> LoadAsync(Guid storeId, Guid articleId, CancellationToken cancellationToken = default);
  Task<IEnumerable<ProductAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IEnumerable<ProductAggregate>> LoadAsync(ArticleAggregate article, CancellationToken cancellationToken = default);
  Task<IEnumerable<ProductAggregate>> LoadAsync(StoreAggregate store, CancellationToken cancellationToken = default);
  Task<IEnumerable<ProductAggregate>> LoadAsync(StoreAggregate store, NumberUnit departmentNumber, CancellationToken cancellationToken = default);
  Task<ProductAggregate?> LoadAsync(StoreId storeId, SkuUnit sku, CancellationToken cancellationToken = default);
  Task SaveAsync(ProductAggregate product, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ProductAggregate> products, CancellationToken cancellationToken = default);
}
