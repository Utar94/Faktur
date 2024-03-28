using Faktur.Contracts.Departments;
using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Application.Departments.Validators;

internal class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentPayload>
{
  public UpdateDepartmentValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));
  }
}
