namespace Faktur.Models.Account;

public record SignInPayload : Credentials
{
  public SignInPayload() : base()
  {
  }

  public SignInPayload(string username, string password) : base(username, password)
  {
  }
}
