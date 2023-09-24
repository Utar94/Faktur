using Logitar.Faktur.Contracts.Search;

namespace Logitar.Faktur.Contracts.Stores;

public record SearchStoresPayload : SearchPayload
{
  public new List<StoreSortOption> Sort { get; set; } = new();
}
