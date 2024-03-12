using Faktur.Contracts.Banners;
using Faktur.Domain.Shared;
using FluentValidation;

namespace Faktur.Application.Banners.Validators;

internal class ReplaceBannerValidator : AbstractValidator<ReplaceBannerPayload>
{
  public ReplaceBannerValidator()
  {
    RuleFor(x => x.DisplayName).SetValidator(new DisplayNameValidator());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }
}
