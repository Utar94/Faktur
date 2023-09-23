using FluentValidation;
using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Application.Articles.Validators;

internal class ReplaceArticlePayloadValidator : AbstractValidator<ReplaceArticlePayload>
{
  public ReplaceArticlePayloadValidator()
  {
    When(p => !string.IsNullOrWhiteSpace(p.Gtin),
      () => RuleFor(p => p.Gtin!).SetValidator(new GtinValidator()));

    RuleFor(p => p.DisplayName).SetValidator(new DisplayNameValidator());

    When(p => !string.IsNullOrWhiteSpace(p.Description),
      () => RuleFor(p => p.Description!).SetValidator(new DescriptionValidator()));
  }
}
