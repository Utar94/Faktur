using FluentValidation;
using Logitar.Faktur.Domain.Extensions;

namespace Logitar.Faktur.Domain.Stores;

public class StoreNumberValidator : AbstractValidator<string>
{
  public StoreNumberValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(StoreNumberUnit.MaximumLength)
      .Must(x => x.All(char.IsDigit)) // TODO(fpion): refactor
        .WithErrorCode("StoreNumberValidator")
        .WithMessage("'{PropertyName}' may only contain digits.")
      .WithPropertyName(propertyName);
  }
}
