namespace Faktur.Contracts.Receipts;

public record ReplaceReceiptPayload
{
  public DateTime IssuedOn { get; set; }
  public string? Number { get; set; }
}
