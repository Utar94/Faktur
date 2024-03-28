using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Domain.Shared;

public record DescriptionUnit // TODO(fpion): remove this
{
  public string Value { get; }

  public DescriptionUnit(string value)
  {
    Value = value.Trim();
    new DescriptionValidator().ValidateAndThrow(Value);
  }

  public static DescriptionUnit? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new DescriptionUnit(value);
  }
}
