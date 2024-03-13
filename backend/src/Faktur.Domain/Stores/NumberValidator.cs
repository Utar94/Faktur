using FluentValidation;

namespace Faktur.Domain.Stores;

public class NumberValidator : AbstractValidator<string>
{
  public NumberValidator()
  {
    RuleFor(x => x).NotEmpty().MaximumLength(NumberUnit.MaximumLength);
  }
}
