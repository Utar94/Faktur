using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using FluentValidation;

namespace Faktur.Application.Receipts.Validators;

internal class CreateReceiptValidator : AbstractValidator<CreateReceiptPayload>
{
  public CreateReceiptValidator()
  {
    When(x => x.IssuedOn.HasValue, () => RuleFor(x => x.IssuedOn!.Value).SetValidator(new IssuedOnValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Number), () => RuleFor(x => x.Number!).SetValidator(new NumberValidator()));
  }
}
