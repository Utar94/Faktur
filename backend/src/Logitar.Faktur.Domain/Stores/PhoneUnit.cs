using FluentValidation;
using Logitar.Faktur.Contracts.Stores;

namespace Logitar.Faktur.Domain.Stores;

public record PhoneUnit : IPhone
{
  public const int CountryCodeMaximumLength = 2;
  public const int NumberMaximumLength = 20;
  public const int ExtensionMaximumLength = 10;

  public string? CountryCode { get; }
  public string Number { get; }
  public string? Extension { get; }

  public PhoneUnit(string number, string? countryCode = null, string? extension = null)
  {
    CountryCode = countryCode?.CleanTrim();
    Number = number.Trim();
    Extension = extension?.CleanTrim();

    new PhoneValidator(nameof(PhoneUnit)).ValidateAndThrow(this);
  }
}
