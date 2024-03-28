using Faktur.Domain.Taxes;
using Logitar;

namespace Faktur.Application.Taxes;

public class TaxCodeAlreadyUsedException : Exception
{
  public const string ErrorMessage = "The specified tax code is already used.";

  public string TaxCode
  {
    get => (string)Data[nameof(TaxCode)]!;
    private set => Data[nameof(TaxCode)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public TaxCodeAlreadyUsedException(TaxCodeUnit code, string? propertyName = null) : base(BuildMessage(code, propertyName))
  {
    TaxCode = code.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(TaxCodeUnit code, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TaxCode), code.Value)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
