using Logitar.Identity.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Faktur.Models.Account;

public record ContactInformation
{
  public AddressPayload? Address { get; set; }
  public EmailPayload Email { get; set; }
  public PhonePayload? Phone { get; set; }

  public ContactInformation() : this(new EmailPayload())
  {
  }

  public ContactInformation(EmailPayload email)
  {
    Email = email;
  }

  public void ApplyTo(UpdateUserPayload payload)
  {
    if (Address != null)
    {
      Address.IsVerified = false;
    }
    payload.Address = new Modification<AddressPayload>(Address);

    Email.IsVerified = false;
    payload.Email = new Modification<EmailPayload>(Email);

    if (Phone != null)
    {
      Phone.IsVerified = false;
    }
    payload.Phone = new Modification<PhonePayload>(Phone);
  }
}
