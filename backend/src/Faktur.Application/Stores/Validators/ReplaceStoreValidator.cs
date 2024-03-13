using Faktur.Contracts.Stores;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using FluentValidation;

namespace Faktur.Application.Stores.Validators;

internal class ReplaceStoreValidator : AbstractValidator<ReplaceStorePayload>
{
  public ReplaceStoreValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Number), () => RuleFor(x => x.Number!).SetValidator(new NumberValidator()));
    RuleFor(x => x.DisplayName).SetValidator(new DisplayNameValidator());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }
}
