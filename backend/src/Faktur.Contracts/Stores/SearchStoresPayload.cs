using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Stores;

public record SearchStoresPayload : SearchPayload
{
  public Guid? BannerId { get; set; }

  public new List<StoreSortOption> Sort { get; set; } = [];
}
