﻿using Faktur.Contracts.Banners;
using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Application.Banners.Validators;

internal class UpdateBannerValidator : AbstractValidator<UpdateBannerPayload>
{
  public UpdateBannerValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));
  }
}
