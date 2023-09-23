namespace Faktur.Core.Receipts.Models
{
  public class TaxModel
  {
    public decimal Amount { get; set; }
    public string Code { get; set; } = null!;
    public double Rate { get; set; }
    public decimal TaxableAmount { get; set; }
  }
}
