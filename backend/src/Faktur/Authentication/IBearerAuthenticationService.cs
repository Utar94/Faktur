using Faktur.Models.Account;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;

namespace Faktur.Authentication;

public interface IBearerAuthenticationService
{
  TokenResponse GetTokenResponse(Session session);
  ValidatedToken ValidateAccessToken(string accessToken);
}
