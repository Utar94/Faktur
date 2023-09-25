using FluentValidation;
using Logitar.Faktur.Contracts.Stores;
using Logitar.Faktur.Domain.Stores;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Application.Stores.Validators;

internal class UpdateStorePayloadValidator : AbstractValidator<UpdateStorePayload>
{
  public UpdateStorePayloadValidator()
  {
    When(p => !string.IsNullOrWhiteSpace(p.Number?.Value),
      () => RuleFor(p => p.Number!.Value!).SetValidator(new StoreNumberValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.DisplayName),
      () => RuleFor(p => p.DisplayName!).SetValidator(new DisplayNameValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.Description?.Value),
      () => RuleFor(p => p.Description!.Value!).SetValidator(new DescriptionValidator()));

    When(p => p.Phone?.Value != null,
      () => RuleFor(p => p.Phone!.Value!).SetValidator(new PhoneValidator()));
  }
}
