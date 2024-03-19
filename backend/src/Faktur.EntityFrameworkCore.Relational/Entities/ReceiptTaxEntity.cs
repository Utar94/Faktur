namespace Faktur.EntityFrameworkCore.Relational.Entities;

internal class ReceiptTaxEntity
{
  public ReceiptEntity? Receipt { get; private set; }
  public int ReceiptId { get; private set; }

  public string Code { get; private set; } = string.Empty;
  public double Rate { get; private set; }
  public decimal TaxableAmount { get; private set; }
  public decimal Amount { get; private set; }

  // TODO(fpion): public constructor

  private ReceiptTaxEntity()
  {
  }
}
