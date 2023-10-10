using FluentValidation;

namespace Logitar.Faktur.Domain.Articles;

public record GtinUnit
{
  public const int MaximumLength = 14;

  public string Value { get; }
  public long NormalizedValue => long.Parse(Value);

  public GtinUnit(string value)
  {
    new GtinValidator(nameof(Value)).ValidateAndThrow(value);
    Value = value.Trim();
  }

  public static GtinUnit Parse(string value, string propertyName)
  {
    new GtinValidator(propertyName).ValidateAndThrow(value);

    return new GtinUnit(value);
  }

  public static GtinUnit? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);
}
