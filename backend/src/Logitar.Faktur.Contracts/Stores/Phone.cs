namespace Logitar.Faktur.Contracts.Stores;

public record Phone : IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; } = string.Empty;
  public string? Extension { get; set; }
  public string E164Formatted { get; set; } = string.Empty;
}
