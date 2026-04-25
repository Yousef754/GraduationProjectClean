using ECommerce.Domain.Entities.AppUser;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
        {
            var smtpClient = new SmtpClient(_settings.SmtpHost)
            {
                Port = _settings.SmtpPort,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.From),
                Subject = "Password Reset Request",
                Body = $@"
                <h2>Password Reset</h2>
                <p>You requested to reset your password.</p>
                <p>Your reset token is:</p>
                <h3 style='color: blue;'>{resetToken}</h3>
                <p>This token is valid for <b>1 hour</b>.</p>
                <p>If you didn't request this, please ignore this email.</p>
            ",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
