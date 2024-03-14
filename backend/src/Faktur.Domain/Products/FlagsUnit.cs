using FluentValidation;

namespace Faktur.Domain.Products;

public record FlagsUnit
{
  public const int MaximumLength = 10;

  public string Value { get; }

  public FlagsUnit(string value)
  {
    Value = value.Trim();
    new FlagsValidator().ValidateAndThrow(Value);
  }

  public static FlagsUnit? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new FlagsUnit(value);
  }
}
