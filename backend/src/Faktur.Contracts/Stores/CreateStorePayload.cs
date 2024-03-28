using Logitar.Portal.Contracts.Users;

namespace Faktur.Contracts.Stores;

public record CreateStorePayload
{
  public Guid? BannerId { get; set; }
  public string? Number { get; set; }
  public string DisplayName { get; set; }
  public string? Description { get; set; }

  public AddressPayload? Address { get; set; }
  public EmailPayload? Email { get; set; }
  public PhonePayload? Phone { get; set; }

  public CreateStorePayload() : this(string.Empty)
  {
  }

  public CreateStorePayload(string displayName)
  {
    DisplayName = displayName;
  }
}
