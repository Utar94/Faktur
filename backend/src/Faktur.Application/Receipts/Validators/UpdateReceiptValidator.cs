using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using FluentValidation;

namespace Faktur.Application.Receipts.Validators;

internal class UpdateReceiptValidator : AbstractValidator<UpdateReceiptPayload>
{
  public UpdateReceiptValidator()
  {
    When(x => x.IssuedOn.HasValue, () => RuleFor(x => x.IssuedOn!.Value).SetValidator(new IssuedOnValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Number?.Value), () => RuleFor(x => x.Number!.Value!).SetValidator(new NumberValidator()));
  }
}
