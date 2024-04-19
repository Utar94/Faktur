using Logitar.Portal.Contracts.Users;

namespace Faktur.Models.Account;

public record SaveProfilePayload
{
  public AuthenticationInformation? AuthenticationInformation { get; set; }
  public ContactInformation? ContactInformation { get; set; }
  public PersonalInformation? PersonalInformation { get; set; }

  public UpdateUserPayload? ToUpdatePayload()
  {
    if (AuthenticationInformation == null && ContactInformation == null && PersonalInformation == null)
    {
      return null;
    }

    UpdateUserPayload payload = new();

    AuthenticationInformation?.ApplyTo(payload);
    ContactInformation?.ApplyTo(payload);
    PersonalInformation?.ApplyTo(payload);

    return payload;
  }
}
