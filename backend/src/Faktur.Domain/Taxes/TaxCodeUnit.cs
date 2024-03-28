using FluentValidation;

namespace Faktur.Domain.Taxes;

public record TaxCodeUnit
{
  public const int MaximumLength = 4;

  public string Value { get; }

  public TaxCodeUnit(string value)
  {
    Value = value.Trim();
    new TaxCodeValidator().ValidateAndThrow(Value);
  }

  public static TaxCodeUnit? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new TaxCodeUnit(value);
  }
}
