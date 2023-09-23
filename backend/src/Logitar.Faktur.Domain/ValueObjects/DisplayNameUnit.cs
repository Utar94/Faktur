using FluentValidation;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Domain.ValueObjects;

public record DisplayNameUnit
{
  public const int MaximumLength = 50;

  public string Value { get; }

  public DisplayNameUnit(string value)
  {
    new DisplayNameValidator(nameof(Value)).ValidateAndThrow(value);
    Value = value.Trim();
  }
}
