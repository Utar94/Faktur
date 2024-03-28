using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using FluentValidation;

namespace Faktur.Application.Receipts.Validators;

internal class ReceiptItemCategoryValidator : AbstractValidator<ReceiptItemCategory>
{
  public ReceiptItemCategoryValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Category), () => RuleFor(x => x.Category!).SetValidator(new CategoryUnitValidator()));
  }
}
