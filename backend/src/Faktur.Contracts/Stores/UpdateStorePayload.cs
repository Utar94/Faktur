using Logitar.Portal.Contracts.Users;

namespace Faktur.Contracts.Stores;

public record UpdateStorePayload
{
  public Modification<Guid?>? BannerId { get; set; }
  public Modification<string>? Number { get; set; }
  public string? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public Modification<AddressPayload>? Address { get; set; }
  public Modification<EmailPayload>? Email { get; set; }
  public Modification<PhonePayload>? Phone { get; set; }
}
