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
    <p>Your OTP code is:</p>
    <h1 style='color: blue; letter-spacing: 5px;'>{resetToken}</h1>
    <p>This code is valid for <b>10 minutes</b>.</p>
    <p>If you didn't request this, please ignore this email.</p>
",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
