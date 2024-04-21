namespace Faktur.ETL.Worker.Entities;

internal class ReceiptItemEntity
{
  public int Id { get; set; }
  public Guid Key { get; set; }

  public ProductEntity? Product { get; set; }
  public int ProductId { get; set; }

  public ReceiptEntity? Receipt { get; set; }
  public int ReceiptId { get; set; }

  public double Quantity { get; set; }
  public decimal UnitPrice { get; set; }
  public decimal Price { get; set; }

  public ReceiptLineEntity? Line { get; set; }
}
