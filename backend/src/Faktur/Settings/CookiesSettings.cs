namespace Faktur.Settings;

internal record CookiesSettings
{
  public RefreshTokenCookieSettings RefreshToken { get; set; } = new();
  public SessionCookieSettings Session { get; set; } = new();
}

internal record RefreshTokenCookieSettings
{
  public bool HttpOnly { get; set; } = true;
  public TimeSpan? MaxAge { get; set; } = TimeSpan.FromDays(7);
  public SameSiteMode SameSite { get; set; } = SameSiteMode.Strict;
  public bool Secure { get; set; } = true;
}

internal record SessionCookieSettings
{
  public SameSiteMode SameSite { get; set; } = SameSiteMode.Strict;
}
