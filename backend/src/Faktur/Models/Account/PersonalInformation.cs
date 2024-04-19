using Logitar.Identity.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Faktur.Models.Account;

public record PersonalInformation
{
  public string FirstName { get; set; }
  public string? MiddleName { get; set; }
  public string LastName { get; set; }
  public string? Nickname { get; set; }

  public DateTime? Birthdate { get; set; }
  public string? Gender { get; set; }
  public string? Locale { get; set; }
  public string? TimeZone { get; set; }

  public string? Picture { get; set; }
  public string? Profile { get; set; }
  public string? Website { get; set; }

  public PersonalInformation() : this(string.Empty, string.Empty)
  {
  }

  public PersonalInformation(string firstName, string lastName)
  {
    FirstName = firstName;
    LastName = lastName;
  }

  public void ApplyTo(UpdateUserPayload payload)
  {
    payload.FirstName = new Modification<string>(FirstName);
    payload.MiddleName = new Modification<string>(MiddleName);
    payload.LastName = new Modification<string>(LastName);
    payload.Nickname = new Modification<string>(Nickname);

    payload.Birthdate = new Modification<DateTime?>(Birthdate);
    payload.Gender = new Modification<string>(Gender);
    payload.Locale = new Modification<string>(Locale);
    payload.TimeZone = new Modification<string>(TimeZone);

    payload.Picture = new Modification<string>(Picture);
    payload.Profile = new Modification<string>(Profile);
    payload.Website = new Modification<string>(Website);
  }
}
