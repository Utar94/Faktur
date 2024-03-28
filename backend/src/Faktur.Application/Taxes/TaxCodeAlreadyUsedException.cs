using Faktur.Contracts.Errors;
using Faktur.Domain.Taxes;
using Logitar;

namespace Faktur.Application.Taxes;

public class TaxCodeAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified tax code is already used.";

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

  public override PropertyError Error => new(this.GetErrorCode(), ErrorMessage, TaxCode, PropertyName);

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
