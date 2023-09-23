using System.Text;

namespace Logitar.Faktur.Application.Exceptions;

internal class ExceptionMessageBuilder
{
  private readonly StringBuilder message;

  public ExceptionMessageBuilder()
  {
    message = new();
  }
  public ExceptionMessageBuilder(string message) : this()
  {
    this.message.AppendLine(message);
  }

  public ExceptionMessageBuilder AddData(string key, object? value)
  {
    message.Append(key).Append(": ").Append(value).AppendLine();
    return this;
  }

  public string Build() => message.ToString();
}
