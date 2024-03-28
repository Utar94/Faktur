using Faktur.Contracts.Errors;

namespace Faktur.Application;

public abstract class ConflictException : Exception
{
  public abstract PropertyError Error { get; }

  protected ConflictException(string? message = null, Exception? innerException = null) : base(message, innerException)
  {
  }
}
