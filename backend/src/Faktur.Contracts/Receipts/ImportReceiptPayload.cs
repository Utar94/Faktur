namespace Faktur.Contracts.Receipts;

public record ImportReceiptPayload
{
  public Guid StoreId { get; set; }
  public DateTime? IssuedOn { get; set; }
  public string? Number { get; set; }
  public string? Locale { get; set; }
  public string? Lines { get; set; }
}
