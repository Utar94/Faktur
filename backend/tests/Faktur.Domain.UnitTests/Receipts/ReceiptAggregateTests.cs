using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;

namespace Faktur.Domain.Receipts;

[Trait(Traits.Category, Categories.Unit)]
public class ReceiptAggregateTests
{
  private readonly ReceiptAggregate _receipt;
  private readonly TaxAggregate _gst;
  private readonly TaxAggregate _hst;
  private readonly TaxAggregate _qst;

  public ReceiptAggregateTests()
  {
    StoreAggregate store = new(new DisplayNameUnit("Maxi Drummondville"));
    _receipt = new(store);

    _gst = new(new TaxCodeUnit("GST"), rate: 0.05d)
    {
      Flags = new FlagsUnit("F")
    };
    _hst = new(new TaxCodeUnit("HST"), rate: 0.13d)
    {
      Flags = new FlagsUnit("H")
    };
    _qst = new(new TaxCodeUnit("QST"), rate: 0.09975d)
    {
      Flags = new FlagsUnit("P")
    };
  }

  [Fact(DisplayName = "Calculate: it should calculate the correct taxes.")]
  public void Calculate_it_should_calculate_the_correct_taxes()
  {
    _receipt.SetItem(1, new ReceiptItemUnit(new GtinUnit("06038385904"), sku: null, new DisplayNameUnit("PC POULET BBQ"), new FlagsUnit("FPMRJ"),
      quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber: null, department: null));
    _receipt.SetItem(2, new ReceiptItemUnit(new GtinUnit("05749601228"), sku: null, new DisplayNameUnit("KCALM LAUGH BIO"), new FlagsUnit("FRQ"),
      quantity: 1.0d, unitPrice: 10.29m, price: 9.29m, departmentNumber: null, department: null));
    _receipt.SetItem(3, new ReceiptItemUnit(new GtinUnit("4011"), sku: null, new DisplayNameUnit("BANANES"), new FlagsUnit("MRJ"),
      quantity: 1.150d, unitPrice: 1.52m, price: 1.75m, departmentNumber: null, department: null));

    _receipt.Calculate([_gst, _hst, _qst]);
    Assert.Equal(21.03m, _receipt.SubTotal);

    Assert.Equal(2, _receipt.Taxes.Count);

    Assert.Equal(_gst.Rate, _receipt.Taxes[_gst.Code.Value].Rate);
    Assert.Equal(19.28m, _receipt.Taxes[_gst.Code.Value].TaxableAmount);
    Assert.Equal(0.96m, _receipt.Taxes[_gst.Code.Value].Amount);

    Assert.Equal(_qst.Rate, _receipt.Taxes[_qst.Code.Value].Rate);
    Assert.Equal(9.99m, _receipt.Taxes[_qst.Code.Value].TaxableAmount);
    Assert.Equal(1.00m, _receipt.Taxes[_qst.Code.Value].Amount);

    Assert.Equal(21.03m + 0.96m + 1.00m, _receipt.Total);
  }

  [Fact(DisplayName = "Calculate: it should not calculate any tax when no item is taxable.")]
  public void Calculate_it_should_not_calculate_any_tax_when_no_item_is_taxable()
  {
    _receipt.SetItem(1, new ReceiptItemUnit(new GtinUnit("06038385904"), sku: null, new DisplayNameUnit("PC POULET BBQ"), flags: null,
      quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber: null, department: null));

    _receipt.Calculate([_gst, _qst]);
    Assert.Equal(9.99m, _receipt.SubTotal);
    Assert.Empty(_receipt.Taxes);
    Assert.Equal(9.99m, _receipt.Total);
  }

  [Fact(DisplayName = "Calculate: it should not calculate any tax when none were provided.")]
  public void Calculate_it_should_not_calculate_any_tax_when_none_were_provided()
  {
    _receipt.SetItem(1, new ReceiptItemUnit(new GtinUnit("06038385904"), sku: null, new DisplayNameUnit("PC POULET BBQ"), new FlagsUnit("FPMRJ"),
      quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber: null, department: null));

    _receipt.Calculate([]);
    Assert.Equal(9.99m, _receipt.SubTotal);
    Assert.Empty(_receipt.Taxes);
    Assert.Equal(9.99m, _receipt.Total);
  }

  [Fact(DisplayName = "Calculate: it should not calculate any tax when there is no item.")]
  public void Calculate_it_should_not_calculate_any_tax_when_there_is_no_item()
  {
    _receipt.Calculate([_gst, _qst]);
    Assert.Equal(0m, _receipt.SubTotal);
    Assert.Empty(_receipt.Taxes);
    Assert.Equal(0m, _receipt.Total);
  }
}
