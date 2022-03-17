using Logitar.Validation;

namespace Faktur.Core.Receipts.Payloads
{
  public class SaveTaxPayload
  {
    [MinValue(0)]
    public decimal? TaxableAmount { get; set; }
  }
}
