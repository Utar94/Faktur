using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Stores;

public interface IStoreService
{
  Task<AcceptedCommand> CreateAsync(CreateStorePayload payload, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> DeleteAsync(string id, CancellationToken cancellationToken = default);
  Task<Store?> ReadAsync(string id, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> ReplaceAsync(string id, ReplaceStorePayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Store>> SearchAsync(SearchStoresPayload payload, CancellationToken cancellationToken = default);
  Task<AcceptedCommand> UpdateAsync(string id, UpdateStorePayload payload, CancellationToken cancellationToken = default);
}
