using FluentValidation;

namespace Logitar.Faktur.Domain.Products;

public record SkuUnit
{
  public const int MaximumLength = 32;

  public string Value { get; }

  public SkuUnit(string value)
  {
    new SkuValidator(nameof(Value)).ValidateAndThrow(value);
    Value = value.Trim();
  }

  public static SkuUnit? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);
}
