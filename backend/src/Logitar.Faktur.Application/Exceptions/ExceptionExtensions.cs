namespace Logitar.Faktur.Application.Exceptions;

internal static class ExceptionExtensions
{
  public static string GetErrorCode(this Exception exception) => exception.GetType().Name.Remove(nameof(Exception));
}
