using FluentValidation;
using Logitar.Faktur.Domain.Validators;

namespace Logitar.Faktur.Domain.ValueObjects;

public record DescriptionUnit
{
  public const int MaximumLength = 500;

  public string Value { get; }

  public DescriptionUnit(string value)
  {
    new DescriptionValidator(nameof(Value)).ValidateAndThrow(value);
    Value = value.Trim();
  }

  public static DescriptionUnit? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);
}
