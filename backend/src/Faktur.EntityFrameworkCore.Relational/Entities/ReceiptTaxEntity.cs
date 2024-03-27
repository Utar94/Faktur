using Faktur.Domain.Receipts;

namespace Faktur.EntityFrameworkCore.Relational.Entities;

internal class ReceiptTaxEntity
{
  public ReceiptEntity? Receipt { get; private set; }
  public int ReceiptId { get; private set; }

  public string Code { get; private set; } = string.Empty;
  public double Rate { get; private set; }
  public decimal TaxableAmount { get; private set; }
  public decimal Amount { get; private set; }

  public ReceiptTaxEntity(ReceiptEntity receipt, string code, ReceiptTaxUnit tax)
  {
    Receipt = receipt;
    ReceiptId = receipt.ReceiptId;

    Code = code;

    Update(tax);
  }

  private ReceiptTaxEntity()
  {
  }

  public void Update(ReceiptTaxUnit tax)
  {
    Rate = tax.Rate;
    TaxableAmount = tax.TaxableAmount;
    Amount = tax.Amount;
  }
}
