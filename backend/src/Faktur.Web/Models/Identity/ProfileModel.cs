using Logitar.Identity.Core;

namespace Faktur.Web.Models.Identity
{
  public class ProfileModel
  {
    public ProfileModel(User? user = null)
    {
      if (user != null)
      {
        ConfirmedAt = user.ConfirmedAt;
        CreatedAt = user.CreatedAt;
        Email = user.Email;
        FirstName = user.FirstName;
        FullName = user.FullName;
        LastName = user.LastName;
        Locale = user.Locale;
        PasswordChangedAt = user.PasswordChangedAt;
        Picture = user.Picture;
        UpdatedAt = user.UpdatedAt;
      }
    }

    public DateTimeOffset? ConfirmedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? FullName { get; set; }
    public string? LastName { get; set; }
    public string? Locale { get; set; }
    public DateTimeOffset? PasswordChangedAt { get; set; }
    public string? Picture { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
  }
}
