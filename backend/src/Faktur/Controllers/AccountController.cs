using Faktur.Application.Actors;
using Faktur.Authentication;
using Faktur.Extensions;
using Faktur.Models.Account;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
  private readonly IBearerAuthenticationService _bearerService;
  private readonly IPublisher _publisher;
  private readonly ISessionClient _sessionClient;
  private readonly IUserClient _userClient;

  private new User User => HttpContext.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");

  public AccountController(IBearerAuthenticationService bearerService, IPublisher publisher, ISessionClient sessionClient, IUserClient userClient)
  {
    _bearerService = bearerService;
    _publisher = publisher;
    _sessionClient = sessionClient;
    _userClient = userClient;
  }

  [Authorize]
  [HttpPut("password/change")]
  public async Task<ActionResult<UserProfile>> ChangePasswordAsync([FromBody] ChangePasswordInput password, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = new()
    {
      Password = new ChangePasswordPayload(password.New)
      {
        Current = password.Current
      }
    };
    RequestContext context = new(User.Id.ToString(), cancellationToken);
    User user = await _userClient.UpdateAsync(User.Id, payload, context) ?? throw new InvalidOperationException($"The user 'Id={User.Id}' update returned null.");
    UserProfile profile = new(user);
    return Ok(profile);
  }

  [Authorize]
  [HttpGet("profile")]
  public ActionResult<UserProfile> GetProfile()
  {
    UserProfile profile = new(User);
    return Ok(profile);
  }

  [Authorize]
  [HttpPut("profile")]
  public async Task<ActionResult<UserProfile>> SaveProfileAsync([FromBody] SaveProfilePayload input, CancellationToken cancellationToken)
  {
    UpdateUserPayload? payload = input.ToUpdatePayload();
    if (payload == null)
    {
      return GetProfile();
    }

    RequestContext context = new(User.Id.ToString(), cancellationToken);
    User user = await _userClient.UpdateAsync(User.Id, payload, context) ?? throw new InvalidOperationException($"The user 'Id={User.Id}' update returned null.");
    await _publisher.Publish(new UserUpdatedEvent(user), cancellationToken);
    UserProfile profile = new(user);
    return Ok(profile);
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    Session session = await SignInAsync((Credentials)payload, cancellationToken);
    HttpContext.SignIn(session);
    return Ok(new CurrentUser(session));
  }

  [Authorize]
  [HttpPost("sign/out")]
  public async Task<ActionResult> SignOutAsync(CancellationToken cancellationToken)
  {
    Guid? sessionId = HttpContext.GetSessionId();
    if (sessionId.HasValue)
    {
      RequestContext context = new(User.Id.ToString(), cancellationToken);
      _ = await _sessionClient.SignOutAsync(sessionId.Value, context);
    }
    HttpContext.SignOut();
    return NoContent();
  }

  [Authorize]
  [HttpPost("sign/out/all")]
  public async Task<ActionResult> SignOutAllAsync(CancellationToken cancellationToken)
  {
    RequestContext context = new(User.Id.ToString(), cancellationToken);
    _ = await _userClient.SignOutAsync(User.Id, context);
    HttpContext.SignOut();
    return NoContent();
  }

  [HttpPost("token")]
  public async Task<ActionResult<TokenResponse>> GetTokenAsync([FromBody] TokenRequest request, CancellationToken cancellationToken)
  {
    Session session;
    if (request.Credentials != null)
    {
      session = await SignInAsync(request.Credentials, cancellationToken);
    }
    else if (!string.IsNullOrWhiteSpace(request.RefreshToken))
    {
      RenewSessionPayload payload = new(request.RefreshToken.Trim(), HttpContext.GetSessionAttributes());
      RequestContext context = new(cancellationToken);
      session = await _sessionClient.RenewAsync(payload, context);
    }
    else
    {
      Error error = new(code: "InvalidTokenRequest", message: $"Exactly one of the following must be provided: {nameof(request.Credentials)}, {nameof(request.RefreshToken)}.");
      return BadRequest(error);
    }
    TokenResponse token = _bearerService.GetTokenResponse(session);
    return Ok(token);
  }

  private async Task<Session> SignInAsync(Credentials input, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = new(input.Username, input.Password, isPersistent: true, HttpContext.GetSessionAttributes());
    RequestContext context = new(cancellationToken);
    Session session = await _sessionClient.SignInAsync(payload, context);
    await _publisher.Publish(new UserSignedInEvent(session.User), cancellationToken);
    return session;
  }
}
