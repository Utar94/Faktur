using Logitar.Faktur.Contracts.Stores;
using Logitar.Faktur.Domain.Stores;

namespace Logitar.Faktur.Application.Stores;

internal static class StorePayloadExtensions
{
  public static AddressUnit ToAddressUnit(this AddressPayload payload) => new(payload.Street,
    payload.Locality, payload.Country, payload.Region, payload.PostalCode);
  public static PhoneUnit ToPhoneUnit(this PhonePayload payload) => new(payload.Number, payload.CountryCode, payload.Extension);
}
