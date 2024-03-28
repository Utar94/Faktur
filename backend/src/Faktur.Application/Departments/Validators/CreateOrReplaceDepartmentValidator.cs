using Faktur.Contracts.Departments;
using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Application.Departments.Validators;

internal class CreateOrReplaceDepartmentValidator : AbstractValidator<CreateOrReplaceDepartmentPayload>
{
  public CreateOrReplaceDepartmentValidator()
  {
    RuleFor(x => x.DisplayName).SetValidator(new DisplayNameValidator());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }
}
