using FluentValidation.Results;

namespace Faktur.Domain.Receipts;

[Trait(Traits.Category, Categories.Unit)]
public class ReceiptTaxUnitTests
{
  [Theory(DisplayName = "Calculate: it should calculate the correct receipt tax.")]
  [InlineData(0.05, 9.99, 0.50)]
  public void Calculate_it_should_calculate_the_correct_receipt_tax(double rate, decimal taxableAmount, decimal amount)
  {
    ReceiptTaxUnit tax = ReceiptTaxUnit.Calculate(rate, taxableAmount);
    Assert.Equal(amount, tax.Amount);
  }

  [Theory(DisplayName = "ctor: it should create the correct receipt tax.")]
  [InlineData(0.05, 9.99, 0.50)]
  public void ctor_it_should_create_the_correct_receipt_tax(double rate, decimal taxableAmount, decimal amount)
  {
    ReceiptTaxUnit tax = new(rate, taxableAmount, amount);
    Assert.Equal(rate, tax.Rate);
    Assert.Equal(taxableAmount, tax.TaxableAmount);
    Assert.Equal(amount, tax.Amount);
  }

  [Fact(DisplayName = "ctor: it should throw ValidationException when the receipt tax is not valid.")]
  public void ctor_it_should_throw_ValidationException_when_the_receipt_tax_is_not_valid()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new ReceiptTaxUnit(rate: 0.05, taxableAmount: 9.99m, amount: -0.50m));
    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("GreaterThanValidator", failure.ErrorCode);
    Assert.Equal(nameof(ReceiptTaxUnit.Amount), failure.PropertyName);
  }
}
