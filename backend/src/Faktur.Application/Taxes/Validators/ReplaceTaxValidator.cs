using Faktur.Contracts.Taxes;
using Faktur.Domain.Products;
using Faktur.Domain.Taxes;
using FluentValidation;

namespace Faktur.Application.Taxes.Validators;

internal class ReplaceTaxValidator : AbstractValidator<ReplaceTaxPayload>
{
  public ReplaceTaxValidator()
  {
    RuleFor(x => x.Code).SetValidator(new TaxCodeValidator());
    RuleFor(x => x.Rate).GreaterThan(0.0);

    When(x => !string.IsNullOrWhiteSpace(x.Flags), () => RuleFor(x => x.Flags!).SetValidator(new FlagsValidator()));
  }
}
