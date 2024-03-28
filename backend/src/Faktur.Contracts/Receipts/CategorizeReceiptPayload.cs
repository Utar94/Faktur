namespace Faktur.Contracts.Receipts;

public record CategorizeReceiptPayload
{
  public List<ReceiptItemCategory> ItemCategories { get; set; } = [];
}
