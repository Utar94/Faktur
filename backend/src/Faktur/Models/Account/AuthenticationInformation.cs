using Logitar.Portal.Contracts.Users;

namespace Faktur.Models.Account;

public record AuthenticationInformation
{
  public ChangePasswordInput? Password { get; set; }

  public void ApplyTo(UpdateUserPayload payload)
  {
    if (Password != null)
    {
      payload.Password = new ChangePasswordPayload(Password.New)
      {
        Current = Password.Current
      };
    }
  }
}
