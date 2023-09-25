using FluentValidation;
using Logitar.Faktur.Contracts.Departments;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Application.Departments.Validators;

internal class SaveDepartmentPayloadValidator : AbstractValidator<SaveDepartmentPayload>
{
  public SaveDepartmentPayloadValidator()
  {
    RuleFor(p => p.DisplayName).SetValidator(new DisplayNameValidator());

    When(p => !string.IsNullOrWhiteSpace(p.Description),
      () => RuleFor(p => p.Description!).SetValidator(new DescriptionValidator()));
  }
}
