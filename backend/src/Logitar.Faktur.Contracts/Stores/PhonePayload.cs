namespace Logitar.Faktur.Contracts.Stores;

public record PhonePayload : IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; } = string.Empty;
  public string? Extension { get; set; }
}
