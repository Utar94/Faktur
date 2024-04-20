using FluentValidation;

namespace Faktur.Domain.Receipts;

public class ReceiptTaxValidator : AbstractValidator<ReceiptTaxUnit>
{
  public ReceiptTaxValidator()
  {
    RuleFor(x => x.Rate).GreaterThan(0.0);
    RuleFor(x => x.TaxableAmount).GreaterThanOrEqualTo(0m);
    RuleFor(x => x.Amount).GreaterThanOrEqualTo(0m);
  }
}
