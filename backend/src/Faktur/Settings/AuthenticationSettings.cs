namespace Faktur.Settings;

internal record AuthenticationSettings
{
  public int AccessTokenLifetimeSeconds { get; set; }
  public string Secret { get; set; }
  public string SigningAlgorithm { get; set; }
  public string? Audience { get; set; }
  public string? Issuer { get; set; }

  public AuthenticationSettings() : this(string.Empty, string.Empty)
  {
  }

  public AuthenticationSettings(string secret, string signingAlgorithm)
  {
    Secret = secret;
    SigningAlgorithm = signingAlgorithm;
  }
}
