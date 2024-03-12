using Logitar.Portal.Contracts.Users;

namespace Faktur.Models.Account;

public record UserProfile
{
  public DateTime CreatedOn { get; set; }
  public DateTime UpdatedOn { get; set; }

  public string Username { get; set; }
  public DateTime? PasswordChangedOn { get; set; }

  public string? EmailAddress { get; set; }

  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? FullName { get; set; }

  public string? PictureUrl { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public UserProfile() : this(string.Empty)
  {
  }

  public UserProfile(User user) : this(user.UniqueName)
  {
    CreatedOn = user.CreatedOn;
    UpdatedOn = user.UpdatedOn;

    PasswordChangedOn = user.PasswordChangedOn;

    EmailAddress = user.Email?.Address;

    FirstName = user.FirstName;
    LastName = user.LastName;
    FullName = user.FullName;

    PictureUrl = user.Picture;

    AuthenticatedOn = user.AuthenticatedOn;
  }

  public UserProfile(string username)
  {
    Username = username;
  }
}
