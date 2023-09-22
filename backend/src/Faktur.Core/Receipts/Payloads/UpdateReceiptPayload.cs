using System.ComponentModel.DataAnnotations;

namespace Faktur.Core.Receipts.Payloads
{
  public class UpdateReceiptPayload
  {
    public DateTime IssuedAt { get; set; }

    [StringLength(32)]
    public string? Number { get; set; }
  }
}
