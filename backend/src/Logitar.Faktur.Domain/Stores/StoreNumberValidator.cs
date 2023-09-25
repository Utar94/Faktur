using FluentValidation;
using Logitar.Faktur.Domain.Extensions;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Domain.Stores;

public class StoreNumberValidator : AbstractValidator<string>
{
  public StoreNumberValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(StoreNumberUnit.MaximumLength)
      .SetValidator(new AllowedCharactersValidator("0123456789"))
      .WithPropertyName(propertyName);
  }
}
