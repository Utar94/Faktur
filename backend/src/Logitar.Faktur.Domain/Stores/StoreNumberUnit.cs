using FluentValidation;

namespace Logitar.Faktur.Domain.Stores;

public record StoreNumberUnit
{
  public const int MaximumLength = 9;

  public string Value { get; }
  public int NormalizedValue => int.Parse(Value);

  public StoreNumberUnit(string value)
  {
    new StoreNumberValidator(nameof(Value)).ValidateAndThrow(value);
    Value = value.Trim();
  }

  public static StoreNumberUnit? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);
}
