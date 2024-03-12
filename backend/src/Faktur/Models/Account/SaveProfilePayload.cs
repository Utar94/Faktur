using Logitar.Identity.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Faktur.Models.Account;

public record SaveProfilePayload
{
  public string? EmailAddress { get; set; }

  public string? FirstName { get; set; }
  public string? LastName { get; set; }

  public string? PictureUrl { get; set; }

  public bool HasChanges => !string.IsNullOrWhiteSpace(EmailAddress) || !string.IsNullOrWhiteSpace(PictureUrl)
    || !string.IsNullOrWhiteSpace(FirstName) || !string.IsNullOrWhiteSpace(LastName);

  public UpdateUserPayload? ToUpdatePayload()
  {
    if (!HasChanges)
    {
      return null;
    }

    UpdateUserPayload payload = new();

    if (!string.IsNullOrWhiteSpace(EmailAddress))
    {
      payload.Email = new Modification<EmailPayload>(new EmailPayload(EmailAddress, isVerified: false));
    }

    if (!string.IsNullOrWhiteSpace(FirstName))
    {
      payload.FirstName = new Modification<string>(FirstName);
    }
    if (!string.IsNullOrWhiteSpace(LastName))
    {
      payload.FirstName = new Modification<string>(LastName);
    }

    if (!string.IsNullOrWhiteSpace(PictureUrl))
    {
      payload.Picture = new Modification<string>(PictureUrl);
    }

    return payload;
  }
}
