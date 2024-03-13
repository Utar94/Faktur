using FluentValidation;

namespace Faktur.Domain.Stores;

public class NumberValidator : AbstractValidator<string>
{
  public NumberValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty().MaximumLength(NumberUnit.MaximumLength).WithPropertyName(propertyName);
  }
}
