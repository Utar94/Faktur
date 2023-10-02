using Logitar.Faktur.Application.Extensions;
using Logitar.Faktur.Contracts;
using Logitar.Faktur.Domain.Exceptions;

namespace Logitar.Faktur.Application.Exceptions;

public class TooManyResultsException : Exception
{
  private const string ErrorMessage = "There are too many results.";

  public Type Type
  {
    get
    {
      string typeName = (string)Data[nameof(Type)]!;
      return Type.GetType(typeName) ?? throw new InvalidOperationException($"The type '{typeName}' could not be found.");
    }
    private set => Data[nameof(Type)] = value.GetLongestName();
  }
  public int Expected
  {
    get => (int)Data[nameof(Expected)]!;
    private set => Data[nameof(Expected)] = value;
  }
  public int Actual
  {
    get => (int)Data[nameof(Actual)]!;
    private set => Data[nameof(Actual)] = value;
  }

  public TooManyResultsException(Type type, int expected, int actual)
    : base(BuildMessage(type, expected, actual))
  {
    if (!type.IsSubclassOf(typeof(Aggregate)))
    {
      throw new ArgumentException($"The type must be a subclass of {nameof(Aggregate)}.", nameof(type));
    }

    Type = type;
    Expected = expected;
    Actual = actual;
  }
  private static string BuildMessage(Type type, int expected, int actual) => new ExceptionMessageBuilder(ErrorMessage)
    .AddData(nameof(Type), type.GetLongestName())
    .AddData(nameof(Expected), expected)
    .AddData(nameof(Actual), actual)
    .Build();
}

public class TooManyResultsException<T> : TooManyResultsException where T : Aggregate
{
  public TooManyResultsException(int expected, int actual)
    : base(typeof(T), expected, actual)
  {
  }
}
