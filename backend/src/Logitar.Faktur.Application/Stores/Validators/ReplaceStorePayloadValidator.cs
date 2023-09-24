using FluentValidation;
using Logitar.Faktur.Contracts.Stores;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Application.Stores.Validators;

internal class ReplaceStorePayloadValidator : AbstractValidator<ReplaceStorePayload>
{
  public ReplaceStorePayloadValidator()
  {
    RuleFor(p => p.DisplayName).SetValidator(new DisplayNameValidator());

    When(p => !string.IsNullOrWhiteSpace(p.Description),
      () => RuleFor(p => p.Description!).SetValidator(new DescriptionValidator()));
  }
}
