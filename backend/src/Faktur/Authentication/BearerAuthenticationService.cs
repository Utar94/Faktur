using Faktur.Constants;
using Faktur.Models.Account;
using Faktur.Settings;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Faktur.Authentication;

internal class BearerAuthenticationService : IBearerAuthenticationService
{
  private const string AccessTokenType = "at+jwt";
  private const string TokenType = Schemes.Bearer;

  private readonly AuthenticationSettings _settings;
  private readonly JwtSecurityTokenHandler _tokenHandler = new();

  public BearerAuthenticationService(AuthenticationSettings settings)
  {
    _settings = settings;
    _tokenHandler.InboundClaimTypeMap.Clear();
  }

  public TokenResponse GetTokenResponse(Session session)
  {
    SigningCredentials signingCredentials = new(GetSecurityKey(), _settings.SigningAlgorithm);
    DateTime now = DateTime.UtcNow;
    SecurityTokenDescriptor tokenDescriptor = new()
    {
      Audience = _settings.Audience,
      Expires = now.AddSeconds(_settings.AccessTokenLifetimeSeconds),
      IssuedAt = now,
      Issuer = _settings.Issuer,
      NotBefore = now,
      SigningCredentials = signingCredentials,
      Subject = CreateClaimsIdentity(session),
      TokenType = AccessTokenType
    };
    SecurityToken securityToken = _tokenHandler.CreateToken(tokenDescriptor);
    string accessToken = _tokenHandler.WriteToken(securityToken);

    TokenResponse token = new(accessToken, TokenType)
    {
      ExpiresIn = _settings.AccessTokenLifetimeSeconds,
      RefreshToken = session.RefreshToken
    };
    return token;
  }
  private static ClaimsIdentity CreateClaimsIdentity(Session session)
  {
    ClaimsIdentity identity = new();
    identity.AddClaim(new(Rfc7519ClaimNames.SessionId, session.Id.ToString()));

    User user = session.User;
    identity.AddClaim(new(Rfc7519ClaimNames.Subject, user.Id.ToString()));
    identity.AddClaim(new(Rfc7519ClaimNames.Username, user.UniqueName));

    if (user.Email != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.EmailAddress, user.Email.Address));
      identity.AddClaim(new(Rfc7519ClaimNames.IsEmailVerified, user.Email.IsVerified.ToString().ToLower(), ClaimValueTypes.Boolean));
    }

    if (user.FullName != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.FullName, user.FullName));

      if (user.FirstName != null)
      {
        identity.AddClaim(new(Rfc7519ClaimNames.FirstName, user.FirstName));
      }

      if (user.LastName != null)
      {
        identity.AddClaim(new(Rfc7519ClaimNames.LastName, user.LastName));
      }
    }

    if (user.Picture != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Picture, user.Picture));
    }

    if (user.AuthenticatedOn.HasValue)
    {
      identity.AddClaim(ClaimHelper.Create(Rfc7519ClaimNames.AuthenticationTime, user.AuthenticatedOn.Value));
    }
    return identity;
  }

  public ValidatedToken ValidateAccessToken(string accessToken)
  {
    TokenValidationParameters validationParameters = new()
    {
      IssuerSigningKey = GetSecurityKey(),
      ValidTypes = [AccessTokenType],
      ValidateIssuerSigningKey = true,
      ValidateLifetime = true
    };
    if (_settings.Audience != null)
    {
      validationParameters.ValidAudience = _settings.Audience;
      validationParameters.ValidateAudience = true;
    }
    if (_settings.Issuer != null)
    {
      validationParameters.ValidIssuer = _settings.Issuer;
      validationParameters.ValidateIssuer = true;
    }

    ClaimsPrincipal principal = _tokenHandler.ValidateToken(accessToken, validationParameters, out _);
    return GetValidatedToken(principal);
  }
  private static ValidatedToken GetValidatedToken(ClaimsPrincipal principal)
  {
    ValidatedToken validatedToken = new();
    foreach (Claim claim in principal.Claims)
    {
      string? emailAddress = null;
      Claim? isEmailVerified = null;

      switch (claim.Type)
      {
        case Rfc7519ClaimNames.Subject:
          validatedToken.Subject = claim.Value;
          break;
        case Rfc7519ClaimNames.EmailAddress:
          emailAddress = claim.Value;
          break;
        case Rfc7519ClaimNames.IsEmailVerified:
          isEmailVerified = claim;
          break;
        default:
          validatedToken.Claims.Add(new(claim.Type, claim.Value, claim.ValueType));
          break;
      }

      if (emailAddress != null)
      {
        validatedToken.Email = new Email(emailAddress)
        {
          IsVerified = isEmailVerified != null && bool.Parse(isEmailVerified.Value)
        };
      }
      else if (isEmailVerified != null)
      {
        validatedToken.Claims.Add(new(isEmailVerified.Type, isEmailVerified.Value, isEmailVerified.ValueType));
      }
    }
    return validatedToken;
  }

  private SymmetricSecurityKey GetSecurityKey() => new(Encoding.ASCII.GetBytes(_settings.Secret));
}
