using Faktur.Domain.Articles;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using Faktur.Domain.Taxes;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Domain.Receipts;

[Trait(Traits.Category, Categories.Unit)]
public class ReceiptHelperTests
{
  private readonly TaxAggregate _gst;
  private readonly TaxAggregate _hst;
  private readonly TaxAggregate _qst;

  private readonly StoreAggregate _store;

  public ReceiptHelperTests()
  {
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

    _store = new(new DisplayNameUnit("Maxi Drummondville"));
  }

  [Fact(DisplayName = "Calculate: it should calculate the correct taxes.")]
  public void Calculate_it_should_calculate_the_correct_taxes()
  {
    ReceiptAggregate receipt = new(_store, taxes: [_gst, _hst, _qst]);

    receipt.SetItem(1, new ReceiptItemUnit(new GtinUnit("06038385904"), sku: null, new DisplayNameUnit("PC POULET BBQ"), new FlagsUnit("FPMRJ"),
      quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber: null, department: null));
    receipt.SetItem(2, new ReceiptItemUnit(new GtinUnit("05749601228"), sku: null, new DisplayNameUnit("KCALM LAUGH BIO"), new FlagsUnit("FRQ"),
      quantity: 1.0d, unitPrice: 10.29m, price: 9.29m, departmentNumber: null, department: null));
    receipt.SetItem(3, new ReceiptItemUnit(new GtinUnit("4011"), sku: null, new DisplayNameUnit("BANANES"), new FlagsUnit("MRJ"),
      quantity: 1.150d, unitPrice: 1.52m, price: 1.75m, departmentNumber: null, department: null));

    ReceiptTotal total = ReceiptHelper.Calculate(receipt.Items.Values, receipt.Taxes);

    Assert.Equal(21.03m, total.SubTotal);

    Assert.Equal(3, total.Taxes.Count);

    Assert.Equal(_gst.Rate, total.Taxes[_gst.Code.Value].Rate);
    Assert.Equal(19.28m, total.Taxes[_gst.Code.Value].TaxableAmount);
    Assert.Equal(0.96m, total.Taxes[_gst.Code.Value].Amount);

    Assert.Equal(_hst.Rate, total.Taxes[_hst.Code.Value].Rate);
    Assert.Equal(0.00m, total.Taxes[_hst.Code.Value].TaxableAmount);
    Assert.Equal(0.00m, total.Taxes[_hst.Code.Value].Amount);

    Assert.Equal(_qst.Rate, total.Taxes[_qst.Code.Value].Rate);
    Assert.Equal(9.99m, total.Taxes[_qst.Code.Value].TaxableAmount);
    Assert.Equal(1.00m, total.Taxes[_qst.Code.Value].Amount);

    Assert.Equal(21.03m + 0.96m + 1.00m, total.Total);
  }

  [Fact(DisplayName = "Calculate: it should not calculate any tax when no item is taxable.")]
  public void Calculate_it_should_not_calculate_any_tax_when_no_item_is_taxable()
  {
    ReceiptAggregate receipt = new(_store, taxes: [_gst, _hst, _qst]);

    receipt.SetItem(1, new ReceiptItemUnit(new GtinUnit("06038385904"), sku: null, new DisplayNameUnit("PC POULET BBQ"), flags: null,
      quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber: null, department: null));

    ReceiptTotal total = ReceiptHelper.Calculate(receipt.Items.Values, receipt.Taxes);

    Assert.Equal(9.99m, total.SubTotal);

    Assert.Equal(3, total.Taxes.Count);

    Assert.Equal(_gst.Rate, total.Taxes[_gst.Code.Value].Rate);
    Assert.Equal(0.00m, total.Taxes[_gst.Code.Value].TaxableAmount);
    Assert.Equal(0.00m, total.Taxes[_gst.Code.Value].Amount);

    Assert.Equal(_hst.Rate, total.Taxes[_hst.Code.Value].Rate);
    Assert.Equal(0.00m, total.Taxes[_hst.Code.Value].TaxableAmount);
    Assert.Equal(0.00m, total.Taxes[_hst.Code.Value].Amount);

    Assert.Equal(_qst.Rate, total.Taxes[_qst.Code.Value].Rate);
    Assert.Equal(0.00m, total.Taxes[_qst.Code.Value].TaxableAmount);
    Assert.Equal(0.00m, total.Taxes[_qst.Code.Value].Amount);

    Assert.Equal(9.99m, total.Total);
  }

  [Fact(DisplayName = "Calculate: it should not calculate any tax when none were provided.")]
  public void Calculate_it_should_not_calculate_any_tax_when_none_were_provided()
  {
    ReceiptAggregate receipt = new(_store);

    receipt.SetItem(1, new ReceiptItemUnit(new GtinUnit("06038385904"), sku: null, new DisplayNameUnit("PC POULET BBQ"), new FlagsUnit("FPMRJ"),
      quantity: 1.0d, unitPrice: 9.99m, price: 9.99m, departmentNumber: null, department: null));

    ReceiptTotal total = ReceiptHelper.Calculate(receipt.Items.Values, receipt.Taxes);

    Assert.Equal(9.99m, total.SubTotal);
    Assert.Empty(total.Taxes);
    Assert.Equal(9.99m, total.Total);
  }

  [Fact(DisplayName = "Calculate: it should not calculate any tax when there is no item.")]
  public void Calculate_it_should_not_calculate_any_tax_when_there_is_no_item()
  {
    ReceiptAggregate receipt = new(_store, taxes: [_gst, _hst, _qst]);

    ReceiptTotal total = ReceiptHelper.Calculate(receipt.Items.Values, receipt.Taxes);

    Assert.Equal(0m, total.SubTotal);

    Assert.Equal(3, total.Taxes.Count);

    Assert.Equal(_gst.Rate, total.Taxes[_gst.Code.Value].Rate);
    Assert.Equal(0.00m, total.Taxes[_gst.Code.Value].TaxableAmount);
    Assert.Equal(0.00m, total.Taxes[_gst.Code.Value].Amount);

    Assert.Equal(_hst.Rate, total.Taxes[_hst.Code.Value].Rate);
    Assert.Equal(0.00m, total.Taxes[_hst.Code.Value].TaxableAmount);
    Assert.Equal(0.00m, total.Taxes[_hst.Code.Value].Amount);

    Assert.Equal(_qst.Rate, total.Taxes[_qst.Code.Value].Rate);
    Assert.Equal(0.00m, total.Taxes[_qst.Code.Value].TaxableAmount);
    Assert.Equal(0.00m, total.Taxes[_qst.Code.Value].Amount);

    Assert.Equal(0m, total.Total);
  }
}
