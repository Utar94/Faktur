using FluentValidation;
using Logitar.Faktur.Domain.Extensions;
using Logitar.Faktur.Domain.ValueObjects;

namespace Logitar.Faktur.Domain.Validators;

public class DescriptionValidator : AbstractValidator<string>
{
  public DescriptionValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(DescriptionUnit.MaximumLength)
      .WithPropertyName(propertyName);
  }
}
