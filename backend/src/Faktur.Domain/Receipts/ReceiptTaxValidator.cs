using FluentValidation;

namespace Faktur.Domain.Receipts;

public class ReceiptTaxValidator : AbstractValidator<ReceiptTaxUnit>
{
  public ReceiptTaxValidator()
  {
    RuleFor(x => x.Rate).GreaterThan(0.0);
    RuleFor(x => x.TaxableAmount).GreaterThan(0m);
    RuleFor(x => x.Amount).GreaterThan(0m);
  }
}
