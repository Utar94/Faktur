using FluentValidation;
using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Application.Banners.Validators;

internal class ReplaceBannerPayloadValidator : AbstractValidator<ReplaceBannerPayload>
{
  public ReplaceBannerPayloadValidator()
  {
    RuleFor(p => p.DisplayName).SetValidator(new DisplayNameValidator());

    When(p => !string.IsNullOrWhiteSpace(p.Description),
      () => RuleFor(p => p.Description!).SetValidator(new DescriptionValidator()));
  }
}
