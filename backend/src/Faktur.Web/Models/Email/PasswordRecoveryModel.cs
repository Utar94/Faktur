using Logitar.Identity.Core;
using System.Net;

namespace Faktur.Web.Models.Email
{
  public class PasswordRecoveryModel
  {
    public PasswordRecoveryModel(string baseUrl, RecoverPasswordResult result)
    {
      if (baseUrl == null)
      {
        throw new ArgumentNullException(nameof(baseUrl));
      }
      if (result == null)
      {
        throw new ArgumentNullException(nameof(result));
      }

      var parameters = new Dictionary<string, object?>
      {
        { "id", result.User.Id },
        { "token", WebUtility.UrlEncode(result.Token) }
      };
      if (result.User.Culture != null)
      {
        parameters.Add("locale", result.User.Culture);
      }
      string query = string.Join('&', parameters.Select(pair => $"{pair.Key}={pair.Value}"));

      Link = $"{baseUrl}/user/reset-password?{query}";
      Name = result.User.FullName;
    }

    public string Link { get; }
    public string? Name { get; }
  }
}
