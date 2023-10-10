using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.Exceptions;
using Logitar.Faktur.Domain.Stores;

namespace Logitar.Faktur.Application.Products;

public class ProductNotFoundException : Exception, IFailureException
{
  private const string ErrorMessage = "The specified product could not be found.";

  public StoreId StoreId
  {
    get => new(new AggregateId((string)Data[nameof(StoreId)]!));
    private set => Data[nameof(StoreId)] = value.Value;
  }
  public ArticleId ArticleId
  {
    get => new(new AggregateId((string)Data[nameof(ArticleId)]!));
    private set => Data[nameof(ArticleId)] = value.Value;
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

  public ProductNotFoundException(StoreAggregate store, ArticleId articleId, string propertyName)
    : base(BuildMessage(store, articleId, propertyName))
  {
    StoreId = store.Id;
    ArticleId = articleId;
    PropertyName = propertyName;
  }
  private static string BuildMessage(StoreAggregate store, ArticleId articleId, string propertyName) => new ExceptionMessageBuilder(ErrorMessage)
    .AddData(nameof(StoreId), store.Id.Value)
    .AddData(nameof(ArticleId), articleId.Value)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
