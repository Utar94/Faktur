using FluentValidation;

namespace Logitar.Faktur.Domain.Products;

public record FlagsUnit
{
  public const int MaximumLength = 10;

  public string Value { get; }

  public FlagsUnit(string value)
  {
    new FlagsValidator(nameof(Value)).ValidateAndThrow(value);
    Value = value.Trim();
  }

  public static FlagsUnit? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);
}
