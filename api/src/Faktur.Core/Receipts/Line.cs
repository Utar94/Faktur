namespace Faktur.Core.Receipts
{
  public class Line : Aggregate
  {
    public Line(Guid userId) : base(userId)
    {
    }
    private Line() : base()
    {
    }

    public int ArticleId { get; set; }
    public string ArticleName { get; set; } = null!;
    public int? BannerId { get; set; }
    public string? BannerName { get; set; }
    public string Category { get; set; } = null!;
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? DepartmentNumber { get; set; }
    public string? Flags { get; set; }
    public long? Gtin { get; set; }
    public DateTimeOffset IssuedAt { get; set; }
    public int ItemId { get; set; }
    public decimal Price { get; set; }
    public int ProductId { get; set; }
    public string? ProductLabel { get; set; }
    public double Quantity { get; set; }
    public int ReceiptId { get; set; }
    public string? ReceiptNumber { get; set; }
    public string? Sku { get; set; }
    public int StoreId { get; set; }
    public string StoreName { get; set; } = null!;
    public string? StoreNumber { get; set; }
    public decimal UnitPrice { get; set; }
    public string? UnitType { get; set; }
  }
}
