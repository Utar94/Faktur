namespace Logitar.Faktur.Contracts;

public record ErrorDetail
{
  public string ErrorCode { get; set; }
  public string ErrorMessage { get; set; }

  public ErrorDetail() : this(string.Empty, string.Empty)
  {
  }
  public ErrorDetail(string code, string message)
  {
    ErrorCode = code;
    ErrorMessage = message;
  }
}
