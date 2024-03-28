using Faktur.Contracts.Errors;
using Faktur.Domain.Stores;
using Logitar;

namespace Faktur.Application.Departments;

public class DepartmentNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified department could not be found.";

  public Guid StoreId
  {
    get => (Guid)Data[nameof(StoreId)]!;
    private set => Data[nameof(StoreId)] = value;
  }
  public string DepartmentNumber
  {
    get => (string)Data[nameof(DepartmentNumber)]!;
    private set => Data[nameof(DepartmentNumber)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override PropertyError Error
  {
    get
    {
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, DepartmentNumber, PropertyName);
      error.AddData(nameof(StoreId), StoreId.ToString());
      return error;
    }
  }

  public DepartmentNotFoundException(StoreAggregate store, NumberUnit departmentNumber, string? propertyName = null)
    : base(BuildMessage(store, departmentNumber, propertyName))
  {
    StoreId = store.Id.ToGuid();
    DepartmentNumber = departmentNumber.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(StoreAggregate store, NumberUnit departmentNumber, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(StoreId), store.Id.ToGuid())
    .AddData(nameof(DepartmentNumber), departmentNumber.Value)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
