using Faktur.Contracts.Stores;
using Faktur.Domain.Stores;
using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;

namespace Faktur.Application.Stores.Validators;

internal class UpdateStoreValidator : AbstractValidator<UpdateStorePayload>
{
  public UpdateStoreValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Number?.Value), () => RuleFor(x => x.Number!.Value!).SetValidator(new NumberValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));

    When(x => x.Address?.Value != null, () => RuleFor(x => x.Address!.Value!).SetValidator(new AddressValidator()));
    When(x => x.Email?.Value != null, () => RuleFor(x => x.Email!.Value!).SetValidator(new EmailValidator()));
    When(x => x.Phone?.Value != null, () => RuleFor(x => x.Phone!.Value!).SetValidator(new PhoneValidator()));
  }
}
