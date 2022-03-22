using Faktur.Web.Models.Email;
using Faktur.Web.Settings;
using Logitar.Email;
using Logitar.Identity.Core;
using RazorLight;
using System.Globalization;

namespace Faktur.Web.Email
{
  public class EmailService : IEmailService
  {
    private readonly ApplicationSettings applicationSettings;
    private readonly IMessageService messageService;
    private readonly IRazorLightEngine razorLightEngine;

    public EmailService(
      ApplicationSettings applicationSettings,
      IMessageService messageService,
      IRazorLightEngine razorLightEngine
    )
    {
      this.applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
      this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
      this.razorLightEngine = razorLightEngine ?? throw new ArgumentNullException(nameof(razorLightEngine));
    }

    public async Task SendPasswordRecoveryAsync(RecoverPasswordResult result, CancellationToken cancellationToken = default)
    {
      if (result == null)
      {
        throw new ArgumentNullException(nameof(result));
      }

      if (result.User.Culture != null)
      {
        CultureInfo.CurrentUICulture = result.User.Culture;
      }

      var model = new PasswordRecoveryModel(applicationSettings.BaseUrl, result);
      string body = await razorLightEngine.CompileRenderAsync("Views/Email/PasswordRecovery", model);
      var message = new Message
      {
        Body = body,
        IsBodyHtml = true,
        Subject = Resources.Email.PasswordRecovery_Subject
      };
      message.Recipients.Add(new Recipient(result.User.Email)
      {
        DisplayName = result.User.FullName
      });
      await messageService.SendAsync(message, cancellationToken);
    }

    public async Task SendSignUpAsync(SignUpResult result, CancellationToken cancellationToken = default)
    {
      if (result == null)
      {
        throw new ArgumentNullException(nameof(result));
      }

      if (result.User.Culture != null)
      {
        CultureInfo.CurrentUICulture = result.User.Culture;
      }

      var model = new SignUpModel(applicationSettings.BaseUrl, result);
      string body = await razorLightEngine.CompileRenderAsync("Views/Email/SignUp", model);
      var message = new Message
      {
        Body = body,
        IsBodyHtml = true,
        Subject = Resources.Email.SignUp_Subject
      };
      message.Recipients.Add(new Recipient(result.User.Email)
      {
        DisplayName = result.User.FullName
      });

      await messageService.SendAsync(message, cancellationToken);
    }
  }
}
