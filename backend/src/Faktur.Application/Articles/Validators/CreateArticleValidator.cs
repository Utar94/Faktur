using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Application.Articles.Validators;

internal class CreateArticleValidator : AbstractValidator<CreateArticlePayload>
{
  public CreateArticleValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Gtin), () => RuleFor(x => x.Gtin!).SetValidator(new GtinValidator()));
    RuleFor(x => x.DisplayName).SetValidator(new DisplayNameValidator());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }
}
