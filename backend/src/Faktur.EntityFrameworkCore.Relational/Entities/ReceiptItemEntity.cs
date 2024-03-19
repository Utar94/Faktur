namespace Faktur.EntityFrameworkCore.Relational.Entities;

internal class ReceiptItemEntity
{
  public ReceiptEntity? Receipt { get; private set; }
  public int ReceiptId { get; private set; }

  public int Number { get; private set; }

  public ProductEntity? Product { get; private set; }
  public int? ProductId { get; private set; }

  public double Quantity { get; private set; }
  public decimal Price { get; private set; }

  public string? Gtin { get; private set; }
  public long? GtinNormalized
  {
    get => Gtin == null ? null : long.Parse(Gtin);
    private set { }
  }
  public string? Sku { get; private set; }
  public string? SkuNormalized
  {
    get => Sku?.ToUpper();
    private set { }
  }
  public string Label { get; private set; } = string.Empty;
  public string? Flags { get; private set; }
  public decimal? UnitPrice { get; private set; }

  public string? DepartmentNumber { get; private set; }
  public string? DepartmentName { get; private set; }

  // TODO(fpion): public constructor

  private ReceiptItemEntity()
  {
  }
}
