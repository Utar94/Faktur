namespace Faktur.Core.Receipts.Parsing
{
  public class LineInfo
  {
    public DepartmentInfo? Department { get; set; }
    public string? Flags { get; set; }
    public string Id { get; set; } = null!;
    public string? Label { get; set; }
    public decimal Price { get; set; }
    public double? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
  }
}
