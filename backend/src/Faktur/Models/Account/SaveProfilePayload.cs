using Logitar.Portal.Contracts.Users;

namespace Faktur.Models.Account;

public record SaveProfilePayload
{
  public PersonalInformation? PersonalInformation { get; set; }

  public UpdateUserPayload? ToUpdatePayload()
  {
    if (PersonalInformation == null)
    {
      return null;
    }

    UpdateUserPayload payload = new();

    PersonalInformation?.ApplyTo(payload);

    return payload;
  }
}
