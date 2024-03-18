namespace Faktur.Contracts.Receipts;

public record CreateReceiptPayload
{
  public DateTime? IssuedOn { get; set; }
  public string? Number { get; set; }
}
