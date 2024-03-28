using FluentValidation;
using Logitar.Identity.Domain.Shared;

namespace Faktur.Domain.Shared;

public record DisplayNameUnit // TODO(fpion): remove this
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public DisplayNameUnit(string value)
  {
    Value = value.Trim();
    new DisplayNameValidator().ValidateAndThrow(Value);
  }

  public static DisplayNameUnit? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new DisplayNameUnit(value);
  }
}
