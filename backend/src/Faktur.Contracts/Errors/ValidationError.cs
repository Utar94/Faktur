using Logitar.Portal.Contracts.Errors;

namespace Faktur.Contracts.Errors;

public record ValidationError : Error
{
  public string? AttemptedValue { get; set; }
  public string? PropertyName { get; set; }

  public ValidationError() : this(string.Empty, string.Empty)
  {
  }

  public ValidationError(string code, string message) : base(code, message)
  {
  }
}
