using System.Net;
using System.Net.Mail;

namespace Internal_Portal.Services
{

    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var settings = _config.GetSection("EmailSettings");

            var mail = new MailMessage()
            {
                From = new MailAddress(settings["UserName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            var smtp = new SmtpClient(settings["Host"], int.Parse(settings["Port"]))
            {
                Credentials = new NetworkCredential(settings["UserName"], settings["Password"]),
                EnableSsl = bool.Parse(settings["EnableSSL"])
            };

            await smtp.SendMailAsync(mail);
        }
    }
}
