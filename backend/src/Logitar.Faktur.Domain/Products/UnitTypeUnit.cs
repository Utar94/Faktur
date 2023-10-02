using FluentValidation;

namespace Logitar.Faktur.Domain.Products;

public record UnitTypeUnit
{
  public const int MaximumLength = 4;

  public string Value { get; }

  public UnitTypeUnit(string value)
  {
    new UnitTypeValidator(nameof(Value)).ValidateAndThrow(value);
    Value = value.Trim();
  }

  public static UnitTypeUnit? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);
}
