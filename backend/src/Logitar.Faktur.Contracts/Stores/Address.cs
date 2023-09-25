namespace Logitar.Faktur.Contracts.Stores;

public record Address : IAddress
{
  public string Street { get; set; } = string.Empty;
  public string Locality { get; set; } = string.Empty;
  public string? Region { get; set; }
  public string? PostalCode { get; set; }
  public string Country { get; set; } = string.Empty;
  public string Formatted { get; set; } = string.Empty;
}
