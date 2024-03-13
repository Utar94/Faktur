using Faktur.Contracts.Stores;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using FluentValidation;

namespace Faktur.Application.Stores.Validators;

internal class UpdateStoreValidator : AbstractValidator<UpdateStorePayload>
{
  public UpdateStoreValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Number?.Value), () => RuleFor(x => x.Number!.Value!).SetValidator(new NumberValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));
  }
}
