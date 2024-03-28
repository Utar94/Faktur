using Faktur.Contracts.Errors;
using Faktur.Domain.Products;
using Faktur.Domain.Stores;
using Logitar;

namespace Faktur.Application.Products;

public class SkuAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified Stock Keeping Unit (SKU) is already used.";

  public Guid StoreId
  {
    get => (Guid)Data[nameof(StoreId)]!;
    private set => Data[nameof(StoreId)] = value;
  }
  public string Sku
  {
    get => (string)Data[nameof(Sku)]!;
    private set => Data[nameof(Sku)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override PropertyError Error
  {
    get
    {
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, Sku, PropertyName);
      error.AddData(nameof(StoreId), StoreId.ToString());
      return error;
    }
  }

  public SkuAlreadyUsedException(StoreId storeId, SkuUnit sku, string? propertyName = null) : base(BuildMessage(storeId, sku, propertyName))
  {
    StoreId = storeId.ToGuid();
    Sku = sku.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(StoreId storeId, SkuUnit sku, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(StoreId), storeId.ToGuid())
    .AddData(nameof(Sku), sku.Value)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
