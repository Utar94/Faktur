using Faktur.Models.Account;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;

namespace Faktur.Authentication;

public interface IAuthenticationService
{
  TokenResponse GetTokenResponse(Session session);
  ValidatedToken ValidateAccessToken(string accessToken);
}
