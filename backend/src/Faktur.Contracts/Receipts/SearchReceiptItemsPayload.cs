using Logitar.Portal.Contracts.Search;

namespace Faktur.Contracts.Receipts;

public record SearchReceiptItemsPayload : SearchPayload
{
  public Guid ReceiptId { get; set; }

  public new List<ReceiptItemSortOption> Sort { get; set; } = [];

  public SearchReceiptItemsPayload() : this(Guid.Empty)
  {
  }

  public SearchReceiptItemsPayload(Guid receiptId)
  {
    ReceiptId = receiptId;
  }
}
