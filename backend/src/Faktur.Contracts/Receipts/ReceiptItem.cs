using Faktur.Contracts.Products;

namespace Faktur.Contracts.Receipts;

public record ReceiptItem
{
  public int Number { get; set; }

  public double Quantity { get; set; }
  public decimal Price { get; set; }

  public Product Product { get; set; }

  public ReceiptItem() : this(new Product())
  {
  }

  public ReceiptItem(Product product)
  {
    Product = product;
  }
}
