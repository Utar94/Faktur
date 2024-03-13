using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using Faktur.Domain.Shared;
using FluentValidation;

namespace Faktur.Application.Articles.Validators;

internal class ReplaceArticleValidator : AbstractValidator<ReplaceArticlePayload>
{
  public ReplaceArticleValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Gtin), () => RuleFor(x => x.Gtin!).SetValidator(new GtinValidator()));
    RuleFor(x => x.DisplayName).SetValidator(new DisplayNameValidator());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }
}
