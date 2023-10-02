using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Exceptions;
using Logitar.Faktur.Domain.Products;

namespace Logitar.Faktur.Domain.Stores;

public class SkuAlreadyUsedException : Exception, IFailureException
{
  private const string ErrorMessage = "The specified Stock Keeping Unit (SKU) is already used.";

  public StoreId StoreId
  {
    get => new(new AggregateId((string)Data[nameof(StoreId)]!));
    private set => Data[nameof(StoreId)] = value.Value;
  }
  public SkuUnit Sku
  {
    get => new((string)Data[nameof(Sku)]!);
    private set => Data[nameof(Sku)] = value.Value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, Sku.Value)
  {
    ErrorCode = this.GetErrorCode()
  };

  public SkuAlreadyUsedException(StoreAggregate store, SkuUnit sku, string propertyName)
    : base(BuildMessage(store, sku, propertyName))
  {
    StoreId = store.Id;
    Sku = sku;
    PropertyName = propertyName;
  }
  private static string BuildMessage(StoreAggregate store, SkuUnit sku, string propertyName) => new ExceptionMessageBuilder(ErrorMessage)
    .AddData(nameof(StoreId), store.Id.Value)
    .AddData(nameof(Sku), sku.Value)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
