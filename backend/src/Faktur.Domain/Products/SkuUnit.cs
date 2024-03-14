using FluentValidation;

namespace Faktur.Domain.Products;

public record SkuUnit
{
  public const int MaximumLength = 32;

  public string Value { get; }

  public SkuUnit(string value)
  {
    Value = value.Trim();
    new SkuValidator().ValidateAndThrow(Value);
  }

  public static SkuUnit? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new SkuUnit(value);
  }
}
