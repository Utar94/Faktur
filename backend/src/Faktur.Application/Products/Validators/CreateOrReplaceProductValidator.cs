using Faktur.Contracts.Products;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Application.Products.Validators;

internal class CreateOrReplaceProductValidator : AbstractValidator<CreateOrReplaceProductPayload>
{
  public CreateOrReplaceProductValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.DepartmentNumber), () => RuleFor(x => x.DepartmentNumber!).SetValidator(new NumberValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.Sku), () => RuleFor(x => x.Sku!).SetValidator(new SkuValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Sku!).SetValidator(new DescriptionValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.Flags), () => RuleFor(x => x.Flags!).SetValidator(new FlagsValidator()));

    When(x => x.UnitPrice.HasValue, () => RuleFor(x => x.UnitPrice!.Value).GreaterThan(0));
    When(x => x.UnitType.HasValue, () => RuleFor(x => x.UnitType!.Value).IsInEnum());
  }
}
