using FluentValidation;
using Logitar.Faktur.Domain.Extensions;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Domain.Articles;

public class GtinValidator : AbstractValidator<string>
{
  public GtinValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(GtinUnit.MaximumLength)
      .SetValidator(new AllowedCharactersValidator("0123456789"))
      .WithPropertyName(propertyName);
  }
}
