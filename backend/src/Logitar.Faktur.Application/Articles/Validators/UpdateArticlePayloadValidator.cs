using FluentValidation;
using Logitar.Faktur.Contracts.Articles;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Application.Articles.Validators;

internal class UpdateArticlePayloadValidator : AbstractValidator<UpdateArticlePayload>
{
  public UpdateArticlePayloadValidator()
  {
    When(p => !string.IsNullOrWhiteSpace(p.Gtin?.Value),
      () => RuleFor(p => p.Gtin!.Value!).SetValidator(new GtinValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.DisplayName),
      () => RuleFor(p => p.DisplayName!).SetValidator(new DisplayNameValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.Description?.Value),
      () => RuleFor(p => p.Description!.Value!).SetValidator(new DescriptionValidator()));
  }
}
