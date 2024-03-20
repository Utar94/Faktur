using Faktur.Contracts.Products;
using Faktur.Domain.Products;
using Faktur.Domain.Shared;
using Faktur.Domain.Stores;
using FluentValidation;

namespace Faktur.Application.Products.Validators;

internal class UpdateProductValidator : AbstractValidator<UpdateProductPayload>
{
  public UpdateProductValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.DepartmentNumber?.Value), () => RuleFor(x => x.DepartmentNumber!.Value!).SetValidator(new NumberValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.Sku?.Value), () => RuleFor(x => x.Sku!.Value!).SetValidator(new SkuValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.Flags?.Value), () => RuleFor(x => x.Flags!.Value!).SetValidator(new FlagsValidator()));

    When(x => x.UnitPrice?.Value != null, () => RuleFor(x => x.UnitPrice!.Value!).GreaterThan(0));
    When(x => x.UnitType?.Value != null, () => RuleFor(x => x.UnitType!.Value!).IsInEnum());
  }
}
