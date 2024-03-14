using FluentValidation;

namespace Faktur.Domain.Products;

public class FlagsValidator : AbstractValidator<string>
{
  public FlagsValidator()
  {
    RuleFor(x => x).NotEmpty().MaximumLength(FlagsUnit.MaximumLength);
  }
}
