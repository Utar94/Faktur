using Faktur.Contracts.Receipts;
using Faktur.Domain.Products;
using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Application.Receipts.Validators;

internal class CreateOrReplaceReceiptItemValidator : AbstractValidator<CreateOrReplaceReceiptItemPayload>
{
  public CreateOrReplaceReceiptItemValidator()
  {
    RuleFor(x => x.GtinOrSku).SetValidator(new SkuValidator());
    RuleFor(x => x.Label).SetValidator(new DisplayNameValidator());

    When(x => !string.IsNullOrWhiteSpace(x.Flags), () => RuleFor(x => x.Flags!).SetValidator(new FlagsValidator()));

    When(x => x.Quantity.HasValue, () => RuleFor(x => x.Quantity!.Value).GreaterThan(0));
    When(x => x.UnitPrice.HasValue, () => RuleFor(x => x.UnitPrice!.Value).GreaterThan(0));
    RuleFor(x => x.Price).GreaterThan(0);

    When(x => x.Department != null, () => RuleFor(x => x.Department!).SetValidator(new DepartmentValidator()));
  }
}
