namespace Faktur.Contracts.Errors;

public record ValidationError
{
  public string Code { get; set; }
  public string Message { get; set; }
  public List<PropertyError> Errors { get; set; }

  public ValidationError() : this("Validation", $"Validation failed. See {nameof(Errors)} for more details.")
  {
  }

  public ValidationError(string code, string message)
  {
    Code = code;
    Message = message;
    Errors = [];
  }

  public ValidationError(string code, string message, IEnumerable<PropertyError> errors) : this(code, message)
  {
    Errors.AddRange(errors);
  }
}
