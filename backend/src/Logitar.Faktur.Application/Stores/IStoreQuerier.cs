using Logitar.Faktur.Contracts.Search;
using Logitar.Faktur.Contracts.Stores;

namespace Logitar.Faktur.Application.Stores;

public interface IStoreQuerier
{
  Task<Store?> ReadAsync(string id, CancellationToken cancellationToken = default);
  Task<SearchResults<Store>> SearchAsync(SearchStoresPayload payload, CancellationToken cancellationToken = default);
}
