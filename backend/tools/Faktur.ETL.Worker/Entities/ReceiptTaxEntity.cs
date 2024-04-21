namespace Faktur.ETL.Worker.Entities;

internal class ReceiptTaxEntity
{
  public ReceiptEntity? Receipt { get; set; }
  public int ReceiptId { get; set; }

  public string Code { get; set; } = string.Empty;
  public double Rate { get; set; }
  public decimal TaxableAmount { get; set; }
  public decimal Amount { get; set; }
}
