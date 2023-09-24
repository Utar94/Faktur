using FluentValidation;
using Logitar.Faktur.Contracts.Banners;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Application.Banners.Validators;

internal class CreateBannerPayloadValidator : AbstractValidator<CreateBannerPayload>
{
  public CreateBannerPayloadValidator()
  {
    When(p => !string.IsNullOrWhiteSpace(p.Id),
      () => RuleFor(p => p.Id!).SetValidator(new AggregateIdValidator()));

    RuleFor(p => p.DisplayName).SetValidator(new DisplayNameValidator());

    When(p => !string.IsNullOrWhiteSpace(p.Description),
      () => RuleFor(p => p.Description!).SetValidator(new DescriptionValidator()));
  }
}
