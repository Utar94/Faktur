using Faktur.Contracts.Receipts;
using Faktur.Domain.Stores;
using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Application.Receipts.Validators;

internal class DepartmentValidator : AbstractValidator<DepartmentSummary>
{
  public DepartmentValidator()
  {
    RuleFor(x => x.Number).SetValidator(new NumberValidator());
    RuleFor(x => x.DisplayName).SetValidator(new DisplayNameValidator());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }
}
