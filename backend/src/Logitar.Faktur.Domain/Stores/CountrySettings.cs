namespace Logitar.Faktur.Domain.Stores;

public record CountrySettings
{
  public string? PostalCode { get; init; }
  public HashSet<string>? Regions { get; init; }
}
