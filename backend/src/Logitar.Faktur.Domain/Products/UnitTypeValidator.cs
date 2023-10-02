using FluentValidation;
using Logitar.Faktur.Domain.Extensions;

namespace Logitar.Faktur.Domain.Products;

public class UnitTypeValidator : AbstractValidator<string>
{
  public UnitTypeValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(UnitTypeUnit.MaximumLength)
      .WithPropertyName(propertyName);
  }
}
