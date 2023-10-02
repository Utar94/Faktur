using FluentValidation;
using Logitar.Faktur.Domain.Extensions;

namespace Logitar.Faktur.Domain.Products;

public class FlagsValidator : AbstractValidator<string>
{
  public FlagsValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(FlagsUnit.MaximumLength)
      .WithPropertyName(propertyName);
  }
}
