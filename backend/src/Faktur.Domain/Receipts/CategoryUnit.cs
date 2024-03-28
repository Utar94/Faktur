using FluentValidation;

namespace Faktur.Domain.Receipts;

public record CategoryUnit
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public CategoryUnit(string value)
  {
    Value = value.Trim();
    new CategoryUnitValidator().ValidateAndThrow(Value);
  }

  public static CategoryUnit? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new CategoryUnit(value);
  }
}
