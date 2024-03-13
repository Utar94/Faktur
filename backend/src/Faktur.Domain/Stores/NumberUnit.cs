using FluentValidation;

namespace Faktur.Domain.Stores;

public record NumberUnit
{
  public const int MaximumLength = 16;

  public string Value { get; }

  public NumberUnit(string value)
  {
    Value = value.Trim();
    new NumberValidator().ValidateAndThrow(Value);
  }

  public static NumberUnit? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new NumberUnit(value);
  }
}
