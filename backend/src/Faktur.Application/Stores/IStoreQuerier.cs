using Faktur.Contracts.Stores;
using Faktur.Domain.Stores;
using Logitar.Portal.Contracts.Search;

namespace Faktur.Application.Stores;

public interface IStoreQuerier
{
  Task<Store> ReadAsync(StoreAggregate store, CancellationToken cancellationToken = default);
  Task<Store?> ReadAsync(StoreId id, CancellationToken cancellationToken = default);
  Task<Store?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<Store>> SearchAsync(SearchStoresPayload payload, CancellationToken cancellationToken = default);
}
