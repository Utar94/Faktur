using Faktur.Core.Products;

namespace Faktur.Core.Receipts
{
  public class Item
  {
    public Item(Product product, Receipt receipt)
    {
      Key = Guid.NewGuid();
      Product = product ?? throw new ArgumentNullException(nameof(product));
      ProductId = product.Id;
      Receipt = receipt ?? throw new ArgumentNullException(nameof(receipt));
      ReceiptId = receipt.Id;
    }
    private Item() : base()
    {
    }

    public int Id { get; set; }
    public Guid Key { get; set; }
    public decimal Price { get; set; }
    public Product? Product { get; set; }
    public int ProductId { get; set; }
    public double? Quantity { get; set; }
    public Receipt? Receipt { get; set; }
    public int ReceiptId { get; set; }
    public decimal UnitPrice { get; set; }
  }
}
