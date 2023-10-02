using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Departments;
using Logitar.Faktur.Domain.Exceptions;
using Logitar.Faktur.Domain.Stores;

namespace Logitar.Faktur.Application.Departments;

public class DepartmentNotFoundException : Exception, IFailureException
{
  private const string ErrorMessage = "The specified department could not be found.";

  public StoreId StoreId
  {
    get => new(new AggregateId((string)Data[nameof(StoreId)]!));
    private set => Data[nameof(StoreId)] = value.Value;
  }
  public DepartmentNumberUnit Number
  {
    get => new((string)Data[nameof(Number)]!);
    private set => Data[nameof(Number)] = value.Value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, attemptedValue: null)
  {
    ErrorCode = this.GetErrorCode()
  };

  public DepartmentNotFoundException(StoreAggregate store, DepartmentNumberUnit number, string propertyName)
    : base(BuildMessage(store, number, propertyName))
  {
    StoreId = store.Id;
    Number = number;
    PropertyName = propertyName;
  }
  private static string BuildMessage(StoreAggregate store, DepartmentNumberUnit number, string propertyName) => new ExceptionMessageBuilder(ErrorMessage)
    .AddData(nameof(StoreId), store.Id.Value)
    .AddData(nameof(Number), number.Value)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
