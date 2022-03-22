using Faktur.Web.Email;
using Faktur.Web.Models.Identity;
using Logitar.AspNetCore.Identity;
using Logitar.Identity.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faktur.Web.Controllers
{
  [ApiController]
  [Route("identity")]
  public class IdentityController : ControllerBase
  {
    private readonly IEmailService emailService;
    private readonly IIdentityService identityService;

    public IdentityController(IEmailService emailService, IIdentityService identityService)
    {
      this.emailService = emailService;
      this.identityService = identityService;
    }

    [HttpPost("confirm")]
    public async Task<ActionResult> ConfirmAsync(
      [FromBody] ConfirmPayload payload,
      CancellationToken cancellationToken
    )
    {
      await identityService.ConfirmAsync(payload, cancellationToken);

      return NoContent();
    }

    [Authorize]
    [HttpPost("password/change")]
    public async Task<ActionResult> ChangePasswordAsync(
      [FromBody] ChangePasswordPayload payload,
      CancellationToken cancellationToken
    )
    {
      await identityService.ChangePasswordAsync(payload, cancellationToken);

      return NoContent();
    }

    [HttpPost("password/recover")]
    public async Task<ActionResult> RecoverPasswordAsync(
      [FromBody] RecoverPasswordPayload payload,
      CancellationToken cancellationToken
    )
    {
      RecoverPasswordResult result = await identityService.RecoverPasswordAsync(payload, cancellationToken);

      await emailService.SendPasswordRecoveryAsync(result, cancellationToken);

      return NoContent();
    }

    [HttpPost("password/reset")]
    public async Task<ActionResult> ResetPasswordAsync(
      [FromBody] ResetPasswordPayload payload,
      CancellationToken cancellationToken
    )
    {
      await identityService.ResetPasswordAsync(payload, cancellationToken);

      return NoContent();
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<ProfileModel>> GetProfileAsync(CancellationToken cancellationToken)
    {
      User user = await identityService.GetUserAsync(cancellationToken);

      return Ok(new ProfileModel(user));
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<ProfileModel>> SaveProfileAsync(
      [FromBody] SaveUserPayload payload,
      CancellationToken cancellationToken
    )
    {
      User user = await identityService.SaveUserAsync(payload, cancellationToken);

      return Ok(new ProfileModel(user));
    }

    [HttpPost("renew")]
    public async Task<ActionResult<TokenModel>> RenewAsync(
      [FromBody] RenewPayload payload,
      CancellationToken cancellationToken
    )
    {
      return Ok(await identityService.RenewAsync(payload, cancellationToken));
    }

    [HttpPost("sign/in")]
    public async Task<ActionResult<TokenModel>> SignInAsync(
      [FromBody] SignInPayload payload,
      CancellationToken cancellationToken
    )
    {
      return Ok(await identityService.SignInAsync(payload, cancellationToken));
    }

    [Authorize]
    [HttpPost("sign/out")]
    public async Task<ActionResult> SignOutAsync(
      [FromBody] SignOutPayload payload,
      CancellationToken cancellationToken
    )
    {
      await identityService.SignOutAsync(payload, cancellationToken);

      return NoContent();
    }

    [HttpPost("sign/up")]
    public async Task<ActionResult> SignUpAsync(
      [FromBody] SignUpPayload payload,
      CancellationToken cancellationToken
    )
    {
      SignUpResult result = await identityService.SignUpAsync(payload, cancellationToken: cancellationToken);

      if (result.Token != null)
      {
        //await emailService.SendSignUpAsync(result, cancellationToken); // TODO(fpion): implement
      }

      return NoContent();
    }
  }
}
