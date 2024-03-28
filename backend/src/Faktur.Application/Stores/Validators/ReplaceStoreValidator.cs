using Faktur.Contracts.Stores;
using Faktur.Domain.Stores;
using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;

namespace Faktur.Application.Stores.Validators;

internal class ReplaceStoreValidator : AbstractValidator<ReplaceStorePayload>
{
  public ReplaceStoreValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Number), () => RuleFor(x => x.Number!).SetValidator(new NumberValidator()));
    RuleFor(x => x.DisplayName).SetValidator(new DisplayNameValidator());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));

    When(x => x.Address != null, () => RuleFor(x => x.Address!).SetValidator(new AddressValidator()));
    When(x => x.Email != null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
    When(x => x.Phone != null, () => RuleFor(x => x.Phone!).SetValidator(new PhoneValidator()));
  }
}
