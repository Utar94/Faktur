namespace Faktur.Models.Account;

public abstract record Credentials
{
  public string Username { get; set; }
  public string Password { get; set; }

  protected Credentials() : this(string.Empty, string.Empty)
  {
  }

  protected Credentials(string username, string password)
  {
    Username = username;
    Password = password;
  }
}
