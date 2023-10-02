using Logitar.Faktur.Contracts;

namespace Logitar.Faktur.Domain.Exceptions;

public static class ExceptionExtensions
{
  public static string GetErrorCode(this Exception exception) => exception.GetType().Name.Remove(nameof(Exception));
  public static ErrorDetail GetErrorDetail(this Exception exception) => new()
  {
    ErrorCode = exception.GetErrorCode(),
    ErrorMessage = exception.Message
  };
}
