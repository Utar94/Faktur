using FluentValidation;
using Logitar.Faktur.Contracts.Products;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.Products;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Application.Products.Validators;

internal class UpdateProductPayloadValidator : AbstractValidator<UpdateProductPayload>
{
  public UpdateProductPayloadValidator()
  {
    When(p => !string.IsNullOrWhiteSpace(p.DepartmentNumber?.Value),
      () => RuleFor(x => x.DepartmentNumber!.Value!).SetValidator(new DepartmentNumberValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.Sku?.Value),
      () => RuleFor(p => p.Sku!.Value!).SetValidator(new SkuValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.DisplayName),
      () => RuleFor(p => p.DisplayName!).SetValidator(new DisplayNameValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.Description?.Value),
      () => RuleFor(p => p.Description!.Value!).SetValidator(new DescriptionValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.Flags?.Value),
      () => RuleFor(p => p.Flags!.Value!).SetValidator(new FlagsValidator()));

    When(p => p.UnitPrice?.Value.HasValue == true,
      () => RuleFor(p => p.UnitPrice!.Value!.Value).SetValidator(new UnitPriceValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.UnitType?.Value),
      () => RuleFor(p => p.UnitType!.Value!).SetValidator(new UnitTypeValidator()));
  }
}
