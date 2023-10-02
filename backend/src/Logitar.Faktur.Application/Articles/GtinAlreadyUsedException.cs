using FluentValidation.Results;
using Logitar.Faktur.Domain.Articles;
using Logitar.Faktur.Domain.Exceptions;

namespace Logitar.Faktur.Application.Articles;

public class GtinAlreadyUsedException : Exception, IFailureException
{
  private const string ErrorMessage = "The specified Global Trade Item Number (GTIN) is already used.";

  public GtinUnit Gtin
  {
    get => new((string)Data[nameof(Gtin)]!);
    private set => Data[nameof(Gtin)] = value.Value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, Gtin.Value)
  {
    ErrorCode = this.GetErrorCode()
  };

  public GtinAlreadyUsedException(GtinUnit gtin, string propertyName) : base(BuildMessage(gtin, propertyName))
  {
    Gtin = gtin;
    PropertyName = propertyName;
  }
  private static string BuildMessage(GtinUnit gtin, string propertyName) => new ExceptionMessageBuilder(ErrorMessage)
    .AddData(nameof(Gtin), gtin.Value)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
