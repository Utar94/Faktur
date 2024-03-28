using Faktur.Contracts.Errors;
using Logitar;
using Logitar.EventSourcing;

namespace Faktur.Application;

public class AggregateNotFoundException : NotFoundException
{
  protected virtual string ErrorMessage { get; } = "The specified aggregate could not be found.";

  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }
  public AggregateId AggregateId
  {
    get => new((string)Data[nameof(AggregateId)]!);
    private set => Data[nameof(AggregateId)] = value.Value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override string Message => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TypeName), TypeName)
    .AddData(nameof(AggregateId), AggregateId)
    .AddData(nameof(PropertyName), PropertyName, "<null>")
    .Build();

  public override PropertyError Error => new(this.GetErrorCode(), ErrorMessage, AggregateId.Value, PropertyName);

  public AggregateNotFoundException(Type type, AggregateId aggregateId, string? propertyName = null) : base()
  {
    if (!type.IsSubclassOf(typeof(AggregateRoot)))
    {
      throw new ArgumentException($"The type must be a subclass of '{nameof(AggregateRoot)}'.", nameof(type));
    }

    TypeName = type.GetNamespaceQualifiedName();
    AggregateId = aggregateId;
    PropertyName = propertyName;
  }
}

public class AggregateNotFoundException<T> : AggregateNotFoundException where T : AggregateRoot
{
  public AggregateNotFoundException(AggregateId aggregateId, string? propertyName = null) : base(typeof(T), aggregateId, propertyName)
  {
  }
}
