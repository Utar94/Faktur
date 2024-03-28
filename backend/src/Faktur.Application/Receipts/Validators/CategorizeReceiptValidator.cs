using Faktur.Contracts.Receipts;
using FluentValidation;

namespace Faktur.Application.Receipts.Validators;

internal class CategorizeReceiptValidator : AbstractValidator<CategorizeReceiptPayload>
{
  public CategorizeReceiptValidator()
  {
    RuleForEach(x => x.ItemCategories).SetValidator(new ReceiptItemCategoryValidator());
  }
}
