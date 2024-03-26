using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Receipts;

public record SearchReceiptsPayload : SearchPayload
{
  public Guid? StoreId { get; set; }
  public bool? IsEmpty { get; set; }
  public bool? HasBeenProcessed { get; set; }

  public new List<ReceiptSortOption> Sort { get; set; } = [];
}
