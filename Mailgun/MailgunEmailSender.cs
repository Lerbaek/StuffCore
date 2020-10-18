using System;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Mailgun;
using Mailgun.Services;
using Microsoft.Extensions.Options;
using static System.Environment;

namespace Mailgun
{
  public class MailgunEmailSender : IEmailSender
  {
    public MailgunEmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
    {
      var options = optionsAccessor.Value;
      Validate(nameof(options.MailgunDomain), options.MailgunDomain);
      Validate(nameof(options.MailgunKey),    options.MailgunKey);
      Validate(nameof(options.MailgunRegion), options.MailgunRegion, out MailGunRegion mailgunRegion);

      Email.DefaultSender = new MailgunSender(options.MailgunDomain,
                                              options.MailgunKey,
                                                      mailgunRegion);
    }

    /// <returns>A <see cref="Task"/> of type <see cref="Task"/>&lt;<see cref="SendResponse"/>&gt;</returns>
    public Task SendEmailAsync(string email, string subject, string message) =>
      Email.     From("noreply@lerbaek.dk")
           .       To(email)
           .  Subject(subject)
           .     Body(message, true)
           .SendAsync();

    #region Validation

    private string ExceptionMessage(string option) => $"Please set user secret {option} for project {GetType().Assembly.GetName().Name}";

    private void Validate<TEnum>(string option, string value, out TEnum enumValue) where TEnum : struct, Enum
    {
      if (!Enum.TryParse(value?.ToUpperInvariant(), out enumValue))
        throw new ArgumentException(
          $"{ExceptionMessage(option)} to one of the following:{NewLine}" +
          $"  {string.Join($"{NewLine}  ", Enum.GetNames(typeof(TEnum)))}");
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private void Validate(string option, string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException(ExceptionMessage(option));
    }

    #endregion
  }
}
