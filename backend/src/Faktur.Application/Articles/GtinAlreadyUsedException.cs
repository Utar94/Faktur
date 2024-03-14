using Faktur.Domain.Articles;
using Logitar;

namespace Faktur.Application.Articles;

public class GtinAlreadyUsedException : Exception
{
  private const string ErrorMessage = "The specified Global Trade Item Number (GTIN) is already used.";

  public string Gtin
  {
    get => (string)Data[nameof(Gtin)]!;
    private set => Data[nameof(Gtin)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public GtinAlreadyUsedException(GtinUnit gtin, string? propertyName = null) : base(BuildMessage(gtin, propertyName))
  {
    Gtin = gtin.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(GtinUnit gtin, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Gtin), gtin.Value)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
