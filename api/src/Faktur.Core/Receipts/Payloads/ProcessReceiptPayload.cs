using System.ComponentModel.DataAnnotations;

namespace Faktur.Core.Receipts.Payloads
{
  public class ProcessReceiptPayload
  {
    [Required]
    public Dictionary<string, int[]> Items { get; set; } = new();
  }
}
