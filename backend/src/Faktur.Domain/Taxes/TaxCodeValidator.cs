using FluentValidation;

namespace Faktur.Domain.Taxes;

public class TaxCodeValidator : AbstractValidator<string>
{
  public TaxCodeValidator()
  {
    RuleFor(x => x).NotEmpty().MaximumLength(TaxCodeUnit.MaximumLength);
  }
}
