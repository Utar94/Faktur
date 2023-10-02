using FluentValidation;

namespace Logitar.Faktur.Domain.Products;

public record UnitPriceUnit
{
  public double Value { get; }

  public UnitPriceUnit(double value)
  {
    new UnitPriceValidator(nameof(Value)).ValidateAndThrow(value);
    Value = value;
  }

  public static UnitPriceUnit? TryCreate(double? value) => value.HasValue ? new(value.Value) : null;
}
