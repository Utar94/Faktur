using Faktur.Contracts.Receipts;
using Faktur.Domain.Receipts;
using Faktur.Domain.Stores;
using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Application.Receipts.Validators;

internal class ImportReceiptValidator : AbstractValidator<ImportReceiptPayload>
{
  public ImportReceiptValidator()
  {
    When(x => x.IssuedOn.HasValue, () => RuleFor(x => x.IssuedOn!.Value).SetValidator(new IssuedOnValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Number), () => RuleFor(x => x.Number!).SetValidator(new NumberValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).SetValidator(new LocaleValidator()));
  }
}
