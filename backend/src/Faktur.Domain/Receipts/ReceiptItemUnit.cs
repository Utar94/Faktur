using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;
using FluentValidation;
using Logitar.Identity.Domain.Shared;

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

  public NumberUnit? DepartmentNumber { get; }
  public DepartmentUnit? Department { get; }

  public ReceiptItemUnit(GtinUnit? gtin, SkuUnit? sku, DisplayNameUnit label, FlagsUnit? flags, double quantity, decimal unitPrice, decimal price, NumberUnit? departmentNumber, DepartmentUnit? department)
  {
    Gtin = gtin;
    Sku = sku;

    Label = label;
    Flags = flags;

    Quantity = quantity;
    UnitPrice = unitPrice;
    Price = price;

    DepartmentNumber = departmentNumber;
    Department = department;

    new ReceiptItemValidator().ValidateAndThrow(this);
  }

  public bool IsTaxable(TaxAggregate tax)
  {
    if (Flags != null && tax.Flags != null)
    {
      foreach (char flag in Flags.Value)
      {
        if (tax.Flags.Value.Contains(flag))
        {
          return true;
        }
      }
    }

    return false;
  }
}
