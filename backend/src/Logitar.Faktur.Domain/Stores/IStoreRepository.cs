using Logitar.Faktur.Domain.Banners;

namespace Logitar.Faktur.Domain.Stores;

public interface IStoreRepository
{
  Task<StoreAggregate?> LoadAsync(StoreId id, CancellationToken cancellationToken = default);
  Task<StoreAggregate?> LoadAsync(StoreId id, long? version, CancellationToken cancellationToken = default);
  Task<IEnumerable<StoreAggregate>> LoadAsync(BannerAggregate banner, CancellationToken cancellationToken = default);
  Task SaveAsync(StoreAggregate store, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<StoreAggregate> stores, CancellationToken cancellationToken = default);
}
