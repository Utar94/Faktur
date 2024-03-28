using Faktur.Contracts.Errors;

namespace Faktur.Application;

public abstract class NotFoundException : Exception
{
  public abstract PropertyError Error { get; }

  protected NotFoundException(string? message = null, Exception? innerException = null) : base(message, innerException)
  {
  }
}
