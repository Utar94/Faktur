namespace Faktur.Models.Account;

public record TokenRequest
{
  public Credentials? Credentials { get; set; }

  [JsonPropertyName("refresh_token")]
  public string? RefreshToken { get; set; }
}
