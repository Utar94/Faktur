using FluentValidation;
using Logitar.Faktur.Domain.Extensions;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Domain.Departments;

public class DepartmentNumberValidator : AbstractValidator<string>
{
  public DepartmentNumberValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(DepartmentNumberUnit.MaximumLength)
      .SetValidator(new AllowedCharactersValidator("0123456789"))
      .WithPropertyName(propertyName);
  }
}
