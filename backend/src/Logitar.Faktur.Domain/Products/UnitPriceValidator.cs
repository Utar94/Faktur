using FluentValidation;
using Logitar.Faktur.Domain.Extensions;

namespace Logitar.Faktur.Domain.Products;

public class UnitPriceValidator : AbstractValidator<double>
{
  public UnitPriceValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotNull()
      .GreaterThan(0.00)
      .WithPropertyName(propertyName);
  }
}
