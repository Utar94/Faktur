using Faktur.Contracts.Taxes;
using Faktur.Domain.Products;
using Faktur.Domain.Taxes;
using FluentValidation;

namespace Faktur.Application.Taxes.Validators;

internal class UpdateTaxValidator : AbstractValidator<UpdateTaxPayload>
{
  public UpdateTaxValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Code), () => RuleFor(x => x.Code!).SetValidator(new TaxCodeValidator()));
    When(x => x.Rate.HasValue, () => RuleFor(x => x.Rate!.Value).SetValidator(new TaxRateValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.Flags?.Value), () => RuleFor(x => x.Flags!.Value!).SetValidator(new FlagsValidator()));
  }
}
