using FluentValidation;
using Logitar.Faktur.Contracts.Stores;

namespace Logitar.Faktur.Domain.Stores;

public record AddressUnit : IAddress
{
  public const int StreetMaximumLength = byte.MaxValue;
  public const int LocalityMaximumLength = 100;
  public const int RegionMaximumLength = 2;
  public const int PostalCodeMaximumLength = 10;
  public const int CountryMaximumLength = 2;

  public string Street { get; }
  public string Locality { get; }
  public string? Region { get; }
  public string? PostalCode { get; }
  public string Country { get; }

  public AddressUnit(string street, string locality, string country, string? region = null, string? postalCode = null)
  {
    Street = street.Trim();
    Locality = locality.Trim();
    Region = region?.CleanTrim();
    PostalCode = postalCode?.CleanTrim();
    Country = country.Trim();

    new AddressValidator().ValidateAndThrow(this);
  }
}
