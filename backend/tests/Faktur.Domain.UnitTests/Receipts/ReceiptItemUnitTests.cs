using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;
using FluentValidation.Results;

namespace Faktur.Domain.Receipts;

[Trait(Traits.Category, Categories.Unit)]
public class ReceiptItemUnitTests
{
  [Fact(DisplayName = "ctor: it should create a new receipt item.")]
  public void ctor_it_should_create_a_new_receipt_item()
  {
    GtinUnit gtin = new("06038385904");
    DisplayNameUnit label = new("PC POULET BBQ");
    FlagsUnit flags = new("FPMRJ");
    double quantity = 1.0d;
    decimal unitPrice = 9.99m;
    decimal price = 9.99m;
    NumberUnit departmentNumber = new("36");
    DepartmentUnit department = new(new DisplayNameUnit("PRET-A-MANGER"));
    ReceiptItemUnit item = new(gtin, sku: null, label, flags, quantity, unitPrice, price, departmentNumber, department);
    Assert.Equal(gtin, item.Gtin);
    Assert.Null(item.Sku);
    Assert.Equal(label, item.Label);
    Assert.Equal(flags, item.Flags);
    Assert.Equal(quantity, item.Quantity);
    Assert.Equal(unitPrice, item.UnitPrice);
    Assert.Equal(price, item.Price);
    Assert.Equal(departmentNumber, item.DepartmentNumber);
    Assert.Equal(department, item.Department);
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException when the receipt item is not valid.")]
  public void ctor_it_should_throw_ValidationException_when_the_receipt_item_is_not_valid()
  {
    DisplayNameUnit label = new("PC POULET BBQ");
    var exception = Assert.Throws<FluentValidation.ValidationException>(
      () => new ReceiptItemUnit(gtin: null, sku: null, label, flags: null, quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber: null, department: null)
    );
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal(nameof(ReceiptItemValidator), error.ErrorCode);
  }

  [Fact(DisplayName = "IsTaxable: it should return false when the receipt item is not taxable.")]
  public void IsTaxable_it_should_return_false_when_the_receipt_item_is_not_taxable()
  {
    GtinUnit gtin = new("06038385904");
    DisplayNameUnit label = new("PC POULET BBQ");
    ReceiptItemUnit itemWithoutFlags = new(gtin, sku: null, label, flags: null, quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber: null, department: null);
    ReceiptItemUnit itemWithFlags = new(gtin, sku: null, label, flags: new FlagsUnit("FPMRJ"), quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber: null, department: null);

    TaxAggregate taxWithoutFlags = new(new TaxCodeUnit("GST"), rate: 0.05);
    TaxAggregate taxWithFlags = new(new TaxCodeUnit("QST"), rate: 0.09975)
    {
      Flags = new FlagsUnit("Q")
    };

    Assert.False(itemWithoutFlags.IsTaxable(taxWithoutFlags));
    Assert.False(itemWithoutFlags.IsTaxable(taxWithFlags));
    Assert.False(itemWithFlags.IsTaxable(taxWithoutFlags));
    Assert.False(itemWithFlags.IsTaxable(taxWithFlags));
  }

  [Fact(DisplayName = "IsTaxable: it should return true when the receipt item is taxable.")]
  public void IsTaxable_it_should_return_true_when_the_receipt_item_is_taxable()
  {
    ReceiptItemUnit item = new(new GtinUnit("06038385904"), sku: null, new DisplayNameUnit("PC POULET BBQ"),
      new FlagsUnit("FPMRJ"), quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber: null, department: null);
    TaxAggregate tax = new(new TaxCodeUnit("QST"), rate: 0.09975)
    {
      Flags = new FlagsUnit("P")
    };
    Assert.True(item.IsTaxable(tax));
  }
}
