using Logitar.Portal.Contracts.Errors;

namespace Faktur.Contracts.Errors;

public record PropertyError : Error
{
  public object? AttemptedValue { get; set; }
  public string? PropertyName { get; set; }

  public PropertyError() : this(string.Empty, string.Empty, null)
  {
  }

  public PropertyError(string code, string message, object? attemptedValue, string? propertyName = null) : base(code, message)
  {
    AttemptedValue = attemptedValue;
    PropertyName = propertyName;
  }
}
