using Logitar.Faktur.Contracts.Stores;

namespace Logitar.Faktur.Domain.Stores;

public static class PostalAddressHelper
{
  private static readonly Dictionary<string, CountrySettings> _countries = new();

  static PostalAddressHelper()
  {
    _countries["CA"] = new CountrySettings
    {
      PostalCode = "[ABCEGHJ-NPRSTVXY]\\d[ABCEGHJ-NPRSTV-Z][ -]?\\d[ABCEGHJ-NPRSTV-Z]\\d$",
      Regions = new HashSet<string>(new[] { "AB", "BC", "MB", "NB", "NL", "NT", "NS", "NU", "ON", "PE", "QC", "SK", "YT" })
    };
  }

  public static IEnumerable<string> SupportedCountries => _countries.Keys;
  public static bool IsSupported(string country) => _countries.ContainsKey(country);

  public static CountrySettings? GetCountry(string country)
    => _countries.TryGetValue(country, out CountrySettings? settings) ? settings : null;

  public static string Format(IAddress address)
  {
    StringBuilder formatted = new();

    string[] lines = address.Street.Remove("\r").Split("\n");
    foreach (string line in lines)
    {
      if (!string.IsNullOrWhiteSpace(line))
      {
        formatted.AppendLine(line.Trim());
      }
    }

    formatted.Append(address.Locality.Trim());
    if (!string.IsNullOrWhiteSpace(address.Region))
    {
      formatted.Append(' ').Append(address.Region.Trim());
    }
    if (!string.IsNullOrWhiteSpace(address.PostalCode))
    {
      formatted.Append(' ').Append(address.PostalCode.Trim());
    }
    formatted.AppendLine();

    formatted.Append(address.Country);

    return formatted.ToString();
  }
}
