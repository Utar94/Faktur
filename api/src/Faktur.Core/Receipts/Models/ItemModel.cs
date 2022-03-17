namespace Faktur.Core.Receipts.Models
{
  public class ItemModel
  {
    public string? Code { get; set; }
    public string? Department { get; set; }
    public string? Flags { get; set; }
    public int Id { get; set; }
    public string Label { get; set; } = null!;
    public decimal Price { get; set; }
    public int ProductId { get; set; }
    public double? Quantity { get; set; }
    public decimal UnitPrice { get; set; }
  }
}
