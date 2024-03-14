using Logitar;

namespace Faktur.Application.Stores;

public class StoreNotFoundException : Exception
{
  private const string ErrorMessage = "The specified store could not be found.";

  public Guid StoreId
  {
    get => (Guid)Data[nameof(StoreId)]!;
    private set => Data[nameof(StoreId)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public StoreNotFoundException(Guid storeId, string? propertyName = null) : base(BuildMessage(storeId, propertyName))
  {
    StoreId = storeId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Guid storeId, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(StoreId), storeId)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
