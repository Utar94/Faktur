using FluentValidation;
using Logitar.Faktur.Contracts.Stores;
using Logitar.Faktur.Domain.Stores;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Application.Stores.Validators;

internal class CreateStorePayloadValidator : AbstractValidator<CreateStorePayload>
{
  public CreateStorePayloadValidator()
  {
    When(p => !string.IsNullOrWhiteSpace(p.Id),
      () => RuleFor(p => p.Id!).SetValidator(new AggregateIdValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.Number),
      () => RuleFor(p => p.Number!).SetValidator(new StoreNumberValidator()));

    RuleFor(p => p.DisplayName).SetValidator(new DisplayNameValidator());

    When(p => !string.IsNullOrWhiteSpace(p.Description),
      () => RuleFor(p => p.Description!).SetValidator(new DescriptionValidator()));

    When(p => p.Phone != null,
      () => RuleFor(p => p.Phone!).SetValidator(new PhoneValidator()));
  }
}
