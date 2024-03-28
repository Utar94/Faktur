using Faktur.Contracts.Receipts;
using Faktur.Domain.Products;
using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Application.Receipts.Validators;

internal class UpdateReceiptItemValidator : AbstractValidator<UpdateReceiptItemPayload>
{
  public UpdateReceiptItemValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.GtinOrSku), () => RuleFor(x => x.GtinOrSku!).SetValidator(new SkuValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Label), () => RuleFor(x => x.Label!).SetValidator(new DisplayNameValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.Flags?.Value), () => RuleFor(x => x.Flags!.Value!).SetValidator(new FlagsValidator()));

    When(x => x.Quantity.HasValue, () => RuleFor(x => x.Quantity!.Value).GreaterThan(0));
    When(x => x.UnitPrice.HasValue, () => RuleFor(x => x.UnitPrice!.Value).GreaterThan(0));
    When(x => x.Price.HasValue, () => RuleFor(x => x.Price!.Value).GreaterThan(0));

    When(x => x.Department?.Value != null, () => RuleFor(x => x.Department!.Value!).SetValidator(new DepartmentValidator()));
  }
}
