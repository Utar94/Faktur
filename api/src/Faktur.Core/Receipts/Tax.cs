namespace Faktur.Core.Receipts
{
  public class Tax
  {
    public decimal Amount { get; set; }
    public string Code { get; set; } = null!;
    public double Rate { get; set; }
    public Receipt? Receipt { get; set; }
    public int ReceiptId { get; set; }
    public decimal TaxableAmount { get; set; }
  }
}
