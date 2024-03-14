using Faktur.Domain.Banners;

namespace Faktur.Domain.Stores;

public interface IStoreRepository
{
  Task<StoreAggregate?> LoadAsync(StoreId id, CancellationToken cancellationToken = default);
  Task<StoreAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<StoreAggregate?> LoadAsync(Guid id, long version, CancellationToken cancellationToken = default);
  Task<IEnumerable<StoreAggregate>> LoadAsync(BannerAggregate banner, CancellationToken cancellationToken = default);
  Task SaveAsync(StoreAggregate store, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<StoreAggregate> stores, CancellationToken cancellationToken = default);
}
