using Logitar.Validation;

namespace Faktur.Core.Receipts.Payloads
{
  public class UpdateItemPayload
  {
    [MinValue(0)]
    public decimal? Price { get; set; }

    [MinValue(0)]
    public double? Quantity { get; set; }

    [MinValue(0)]
    public decimal UnitPrice { get; set; }
  }
}
