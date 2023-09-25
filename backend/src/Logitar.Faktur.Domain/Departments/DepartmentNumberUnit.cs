using FluentValidation;

namespace Logitar.Faktur.Domain.Departments;

public record DepartmentNumberUnit
{
  public const int MaximumLength = 9;

  public string Value { get; }
  public int NormalizedValue => int.Parse(Value);

  public DepartmentNumberUnit(string value)
  {
    new DepartmentNumberValidator(nameof(Value)).ValidateAndThrow(value);
    Value = value.Trim();
  }

  public static DepartmentNumberUnit Parse(string value, string propertyName)
  {
    new DepartmentNumberValidator(propertyName).ValidateAndThrow(value);

    return new DepartmentNumberUnit(value);
  }
}
