using Faktur.Contracts.Articles;
using Faktur.Domain.Articles;
using Faktur.Domain.Shared;
using FluentValidation;

namespace Faktur.Application.Articles.Validators;

internal class UpdateArticleValidator : AbstractValidator<UpdateArticlePayload>
{
  public UpdateArticleValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Gtin?.Value), () => RuleFor(x => x.Gtin!.Value!).SetValidator(new GtinValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));
  }
}
