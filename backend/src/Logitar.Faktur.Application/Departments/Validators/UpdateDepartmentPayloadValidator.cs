using FluentValidation;
using Logitar.Faktur.Contracts.Departments;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Application.Departments.Validators;

internal class UpdateDepartmentPayloadValidator : AbstractValidator<UpdateDepartmentPayload>
{
  public UpdateDepartmentPayloadValidator()
  {
    When(p => !string.IsNullOrWhiteSpace(p.DisplayName),
      () => RuleFor(p => p.DisplayName!).SetValidator(new DisplayNameValidator()));

    When(p => !string.IsNullOrWhiteSpace(p.Description?.Value),
      () => RuleFor(p => p.Description!.Value!).SetValidator(new DescriptionValidator()));
  }
}
