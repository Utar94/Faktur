using FluentValidation;

namespace Faktur.Domain.Articles;

public record GtinUnit
{
  public const int MaximumLength = 14;

  public string Value { get; }

  public GtinUnit(string value)
  {
    Value = value.Trim();
    new GtinValidator().ValidateAndThrow(Value);
  }

  public static GtinUnit? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new GtinUnit(value);
  }
}
