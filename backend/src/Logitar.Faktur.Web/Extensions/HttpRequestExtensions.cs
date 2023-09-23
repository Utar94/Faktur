namespace Logitar.Faktur.Web.Extensions;

internal static class HttpRequestExtensions
{
  public static Uri GetBaseUrl(this HttpRequest request) => new($"{request.Scheme}://{request.Host}");
}
