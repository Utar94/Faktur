namespace Faktur.Core.Receipts
{
  public class Tax
  {
    public Tax(Receipt receipt)
    {
      Receipt = receipt ?? throw new ArgumentNullException(nameof(receipt));
      ReceiptId = receipt.Id;
    }
    private Tax()
    {
    }

    public decimal Amount
    {
      get => Math.Round(TaxableAmount * (decimal)Rate, 2);
      set
      {
      }
    }
    public string Code { get; set; } = null!;
    public double Rate { get; set; }
    public Receipt? Receipt { get; set; }
    public int ReceiptId { get; set; }
    public decimal TaxableAmount { get; set; }
  }
}
