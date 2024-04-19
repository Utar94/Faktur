using Logitar.Portal.Contracts.Users;

namespace Faktur.Models.Account;

public record SaveProfilePayload
{
  public ContactInformation? ContactInformation { get; set; }
  public PersonalInformation? PersonalInformation { get; set; }

  public UpdateUserPayload? ToUpdatePayload()
  {
    if (ContactInformation == null && PersonalInformation == null)
    {
      return null;
    }

    UpdateUserPayload payload = new();

    ContactInformation?.ApplyTo(payload);
    PersonalInformation?.ApplyTo(payload);

    return payload;
  }
}
