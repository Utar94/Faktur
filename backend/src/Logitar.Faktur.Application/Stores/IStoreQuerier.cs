using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Contracts.Stores;
using Logitar.Faktur.Domain.Stores;

namespace Logitar.Faktur.Application.Stores;

public interface IStoreQuerier
{
  Task<Store?> ReadAsync(StoreId id, CancellationToken cancellationToken = default);
  Task<SearchResults<Store>> SearchAsync(SearchStoresPayload payload, CancellationToken cancellationToken = default);
}
