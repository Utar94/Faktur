namespace Faktur.Models.Account;

public record Credentials
{
  public string Username { get; set; }
  public string Password { get; set; }

  public Credentials() : this(string.Empty, string.Empty)
  {
  }

  public Credentials(string username, string password)
  {
    Username = username;
    Password = password;
  }
}
