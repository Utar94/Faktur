using FluentValidation;
using Logitar.Faktur.Contracts.Products;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.Products;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Application.Products.Validators;

internal class SaveProductPayloadValidator : AbstractValidator<SaveProductPayload>
{
  public SaveProductPayloadValidator()
  {
    When(p => !string.IsNullOrWhiteSpace(p.DepartmentNumber),
      () => RuleFor(x => x.DepartmentNumber!).SetValidator(new DepartmentNumberValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.Sku),
      () => RuleFor(p => p.Sku!).SetValidator(new SkuValidator()));

    RuleFor(p => p.DisplayName).SetValidator(new DisplayNameValidator());

    When(p => !string.IsNullOrWhiteSpace(p.Description),
      () => RuleFor(p => p.Description!).SetValidator(new DescriptionValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.Flags),
      () => RuleFor(p => p.Flags!).SetValidator(new FlagsValidator()));

    When(p => p.UnitPrice.HasValue,
      () => RuleFor(p => p.UnitPrice!.Value).SetValidator(new UnitPriceValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.UnitType),
      () => RuleFor(p => p.UnitType!).SetValidator(new UnitTypeValidator()));
  }
}
