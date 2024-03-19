using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using FluentValidation;

namespace Faktur.Application.Receipts.Validators;

internal class ReplaceReceiptValidator : AbstractValidator<ReplaceReceiptPayload>
{
  public ReplaceReceiptValidator()
  {
    RuleFor(x => x.IssuedOn).SetValidator(new IssuedOnValidator());
    When(x => !string.IsNullOrWhiteSpace(x.Number), () => RuleFor(x => x.Number!).SetValidator(new NumberValidator()));
  }
}
