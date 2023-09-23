namespace Logitar.Faktur.Web.Settings;

internal record CorsSettings
{
  public bool AllowAnyOrigin { get; set; }
  public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

  public bool AllowAnyMethod { get; set; }
  public string[] AllowedMethods { get; set; } = Array.Empty<string>();

  public bool AllowAnyHeader { get; set; }
  public string[] AllowedHeaders { get; set; } = Array.Empty<string>();

  public bool AllowCredentials { get; set; }
}
