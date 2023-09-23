using FluentValidation;
using Logitar.Faktur.Domain.Extensions;

namespace Logitar.Faktur.Domain.Articles;

public class GtinValidator : AbstractValidator<string>
{
  public GtinValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(GtinUnit.MaximumLength)
      .Must(x => x.All(char.IsDigit))
        .WithErrorCode("GtinValidator")
        .WithMessage("'{PropertyName}' may only contain digits.")
      .WithPropertyName(propertyName);
  }
}
