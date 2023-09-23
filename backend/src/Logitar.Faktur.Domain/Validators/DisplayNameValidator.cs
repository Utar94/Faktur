using FluentValidation;
using Logitar.Faktur.Domain.Extensions;
using Logitar.Faktur.Domain.ValueObjects;

namespace Logitar.Faktur.Domain.Validators;

public class DisplayNameValidator : AbstractValidator<string>
{
  public DisplayNameValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(DisplayNameUnit.MaximumLength)
      .WithPropertyName(propertyName);
  }
}
