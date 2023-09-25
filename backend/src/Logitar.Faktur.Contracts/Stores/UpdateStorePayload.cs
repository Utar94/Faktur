namespace Logitar.Faktur.Contracts.Stores;

public record UpdateStorePayload
{
  public Modification<string>? BannerId { get; set; }

  public Modification<string>? Number { get; set; }
  public string? DisplayName { get; set; } = string.Empty;
  public Modification<string>? Description { get; set; }

  public Modification<AddressPayload>? Address { get; set; }
  public Modification<PhonePayload>? Phone { get; set; }
}
