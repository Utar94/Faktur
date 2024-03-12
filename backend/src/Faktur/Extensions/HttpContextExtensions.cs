using Logitar.Net.Http;

namespace Faktur.Extensions;

internal static class HttpContextExtensions
{
  public static Uri GetBaseUri(this HttpContext context)
  {
    string host = context.Request.Host.Value;
    int index = host.IndexOf(':');

    return new UrlBuilder()
      .SetScheme(context.Request.Scheme, inferPort: true)
      .SetHost(index < 0 ? host : host[..index])
      .BuildUri();
  }
  public static Uri BuildLocation(this HttpContext context, string path, IEnumerable<KeyValuePair<string, string>> parameters)
  {
    UrlBuilder builder = new(context.GetBaseUri());
    builder.SetPath(path);
    foreach (KeyValuePair<string, string> parameter in parameters)
    {
      builder.SetParameter(parameter.Key, parameter.Value);
    }
    return builder.BuildUri();
  }
}
