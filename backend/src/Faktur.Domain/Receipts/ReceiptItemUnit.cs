using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Shared;

namespace Faktur.Domain.Receipts;

public record ReceiptItemUnit
{
  public GtinUnit? Gtin { get; }
  public SkuUnit? Sku { get; }
  public DisplayNameUnit Label { get; }
  public FlagsUnit? Flags { get; }
  public double Quantity { get; }
  public decimal UnitPrice { get; }
  public decimal Price { get; }

  public ReceiptItemUnit(GtinUnit gtin, DisplayNameUnit label, FlagsUnit? flags, decimal price)
    : this(gtin, label, flags, quantity: 1.0, unitPrice: price, price)
  {
  }
  public ReceiptItemUnit(GtinUnit gtin, DisplayNameUnit label, FlagsUnit? flags, double quantity, decimal unitPrice, decimal price)
    : this(label, flags, quantity, unitPrice, price)
  {
    Gtin = gtin;
  }

  public ReceiptItemUnit(SkuUnit sku, DisplayNameUnit label, FlagsUnit? flags, decimal price)
    : this(sku, label, flags, quantity: 1.0, unitPrice: price, price)
  {
  }
  public ReceiptItemUnit(SkuUnit sku, DisplayNameUnit label, FlagsUnit? flags, double quantity, decimal unitPrice, decimal price)
    : this(label, flags, quantity, unitPrice, price)
  {
    Sku = sku;
  }
  private ReceiptItemUnit(DisplayNameUnit label, FlagsUnit? flags, double quantity, decimal unitPrice, decimal price)
  {
    Label = label;
    Flags = flags;
    Quantity = quantity; // TODO(fpion): validate
    UnitPrice = unitPrice; // TODO(fpion): validate
    Price = price; // TODO(fpion): validate
  }
}
