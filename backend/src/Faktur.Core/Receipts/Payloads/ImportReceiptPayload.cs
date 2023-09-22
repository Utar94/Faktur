using Logitar.Validation;
using System.ComponentModel.DataAnnotations;

namespace Faktur.Core.Receipts.Payloads
{
  public class ImportReceiptPayload
  {
    [CultureInfo]
    public string? Culture { get; set; }

    public DateTime? IssuedAt { get; set; }

    public string? Lines { get; set; }

    [StringLength(32)]
    public string? Number { get; set; }

    public int StoreId { get; set; }
  }
}
