using Logitar.Identity.Core;

namespace Faktur.Web.Email
{
  public interface IEmailService
  {
    Task SendPasswordRecoveryAsync(RecoverPasswordResult result, CancellationToken cancellationToken = default);
    Task SendSignUpAsync(SignUpResult result, CancellationToken cancellationToken = default);
  }
}
