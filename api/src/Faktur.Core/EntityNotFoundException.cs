using Logitar.WebApiToolKit.Core.Exceptions;
using System.Text;

namespace Faktur.Core
{
  public class EntityNotFoundException<T> : NotFoundException
  {
    public EntityNotFoundException(
      int id,
      string? paramName = null,
      string? message = null,
      Exception? innerException = null
    ) : base(paramName, message ?? GetMessage(id.ToString(), paramName), innerException)
    {
      Id = id;
    }

    public int Id { get; }

    private static string GetMessage(string id, string? paramName)
    {
      var message = new StringBuilder();

      message.AppendLine("The specified entity could not be found.");
      message.AppendLine($"{paramName ?? "Id"}: {id}");
      message.AppendLine($"Type: {typeof(T).AssemblyQualifiedName}");

      return message.ToString();
    }
  }
}
