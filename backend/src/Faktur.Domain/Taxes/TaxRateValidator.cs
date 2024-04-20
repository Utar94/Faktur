using FluentValidation;

namespace Faktur.Domain.Taxes;

public class TaxRateValidator : AbstractValidator<double>
{
  public TaxRateValidator(string? propertyName = null)
  {
    RuleFor(x => x).GreaterThan(0).LessThan(1).WithPropertyName(propertyName);
  }
}
