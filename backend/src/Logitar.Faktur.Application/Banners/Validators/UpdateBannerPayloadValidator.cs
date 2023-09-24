using FluentValidation;
using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Application.Banners.Validators;

internal class UpdateBannerPayloadValidator : AbstractValidator<UpdateBannerPayload>
{
  public UpdateBannerPayloadValidator()
  {
    When(p => !string.IsNullOrWhiteSpace(p.DisplayName),
      () => RuleFor(p => p.DisplayName!).SetValidator(new DisplayNameValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.Description?.Value),
      () => RuleFor(p => p.Description!.Value!).SetValidator(new DescriptionValidator()));
  }
}
