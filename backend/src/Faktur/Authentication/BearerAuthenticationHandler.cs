using Faktur.Constants;
using Faktur.Extensions;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Faktur.Authentication;

internal class BearerAuthenticationHandler : AuthenticationHandler<BearerAuthenticationOptions>
{
  private readonly IAuthenticationService _authenticationService;
  private readonly IUserClient _userClient;

  public BearerAuthenticationHandler(IAuthenticationService authenticationService, IUserClient userClient,
    IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
  {
    _authenticationService = authenticationService;
    _userClient = userClient;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (Context.Request.Headers.TryGetValue(Headers.Authorization, out StringValues authorization))
    {
      string? value = authorization.Single();
      if (!string.IsNullOrWhiteSpace(value))
      {
        string[] values = value.Split();
        if (values.Length != 2)
        {
          return AuthenticateResult.Fail($"The Authorization header value is not valid: '{value}'.");
        }
        else if (values[0] == Schemes.Bearer)
        {
          try
          {
            ValidatedToken validatedToken = _authenticationService.ValidateAccessToken(values[1]);
            if (string.IsNullOrWhiteSpace(validatedToken.Subject))
            {
              return AuthenticateResult.Fail($"The '{nameof(validatedToken.Subject)}' claim is required.");
            }

            User? user = await _userClient.ReadAsync(Guid.Parse(validatedToken.Subject));
            if (user == null)
            {
              return AuthenticateResult.Fail($"The user 'Id={validatedToken.Subject}' could not be found.");
            }

            Context.SetUser(user);

            ClaimsPrincipal principal = new(user.CreateClaimsIdentity(Scheme.Name));
            AuthenticationTicket ticket = new(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
          }
          catch (Exception exception)
          {
            return AuthenticateResult.Fail(exception);
          }
        }
      }
    }

    return AuthenticateResult.NoResult();
  }
}
