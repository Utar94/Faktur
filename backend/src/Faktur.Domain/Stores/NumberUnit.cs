using FluentValidation;

namespace Faktur.Domain.Stores;

public record NumberUnit
{
  public const int MaximumLength = 16;

  public string Value { get; }

  public NumberUnit(string value, string? propertyName = null)
  {
    Value = value.Trim();
    new NumberValidator(propertyName).ValidateAndThrow(Value);
  }

  public static NumberUnit? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new NumberUnit(value);
  }
}
