using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Taxes;

public record SearchTaxesPayload : SearchPayload
{
  public char? Flag { get; set; }

  public new List<TaxSortOption> Sort { get; set; } = [];
}
