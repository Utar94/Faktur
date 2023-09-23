using Logitar.EventSourcing;
using Logitar.Faktur.Application.Exceptions;
using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Faktur.EntityFrameworkCore.Relational.Exceptions;

public class EntityNotFoundException : Exception
{
  private const string ErrorMessage = "The specified aggregate could not be found.";

  public Type Type
  {
    get
    {
      string typeName = (string)Data[nameof(Type)]!;
      return Type.GetType(typeName) ?? throw new InvalidOperationException($"The type '{typeName}' could not be found.");
    }
    private set => Data[nameof(Type)] = value.GetLongestName();
  }
  public AggregateId Id
  {
    get => new((string)Data[nameof(Id)]!);
    private set => Data[nameof(Id)] = value.Value;
  }

  public EntityNotFoundException(Type type, AggregateId id) : base(BuildMessage(type, id))
  {
    if (!type.IsSubclassOf(typeof(Entity)))
    {
      throw new ArgumentException($"The type must be a subclass of {nameof(Entity)}.", nameof(type));
    }

    Type = type;
    Id = id;
  }
  private static string BuildMessage(Type type, AggregateId id) => new ExceptionMessageBuilder(ErrorMessage)
    .AddData(nameof(Type), type.GetLongestName())
    .AddData(nameof(Id), id)
    .Build();
}

internal class EntityNotFoundException<T> : EntityNotFoundException where T : Entity
{
  public EntityNotFoundException(AggregateId id) : base(typeof(T), id)
  {
  }
}
