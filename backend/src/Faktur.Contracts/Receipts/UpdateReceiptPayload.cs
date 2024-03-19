namespace Faktur.Contracts.Receipts;

public record UpdateReceiptPayload
{
  public DateTime? IssuedOn { get; set; }
  public Modification<string>? Number { get; set; }
}
