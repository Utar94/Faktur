using FluentValidation;
using Logitar.Faktur.Contracts.Stores;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Application.Stores.Validators;

internal class CreateStorePayloadValidator : AbstractValidator<CreateStorePayload>
{
  public CreateStorePayloadValidator()
  {
    When(p => !string.IsNullOrWhiteSpace(p.Id),
      () => RuleFor(p => p.Id!).SetValidator(new AggregateIdValidator()));

    RuleFor(p => p.DisplayName).SetValidator(new DisplayNameValidator());

    When(p => !string.IsNullOrWhiteSpace(p.Description),
      () => RuleFor(p => p.Description!).SetValidator(new DescriptionValidator()));
  }
}
