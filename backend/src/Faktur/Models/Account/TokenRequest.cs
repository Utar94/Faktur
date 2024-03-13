namespace Faktur.Models.Account;

public record TokenRequest
{
  public Credentials? Credentials { get; set; }
  public string? RefreshToken { get; set; }
}
