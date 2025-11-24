using Internal_Portal.Interface;
using Internal_Portal.Models;
using Internal_Portal.Repository;
using Internal_Portal.Services;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;



namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContact _repo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;


        public ContactController(IContact repo,IEmailService emailService,
        IConfiguration config)
        {
            _repo = repo;
                _emailService = emailService;
        _config = config;
        }

        [HttpPost("SaveMessage")]
        public async Task<IActionResult> SaveMessage(ContactMessage model)
        {
            try
            {
                var result = await _repo.InsertMessage(model);

                if (result <= 0)
                    return StatusCode(500, new { success = false, message = "Failed to save message." });

                // ==============================
                //      SEND MAIL TO ADMIN
                // ==============================
                var adminEmail = _config["EmailSettings:AdminEmail"];

                var adminMsg = new MimeMessage();
                adminMsg.From.Add(new MailboxAddress("Internal Portal", "vidhi.p.ivorytechnolab@gmail.com"));
                adminMsg.To.Add(new MailboxAddress("Admin", adminEmail));
                adminMsg.Subject = "New Contact Form Submission";

                adminMsg.Body = new BodyBuilder
                {
                    HtmlBody = $@"
                        <h3>New Contact Message</h3>
                        <p><b>Name:</b> {model.Name}</p>
                        <p><b>Company:</b> {model.CompanyName}</p>
                        <p><b>Email:</b> {model.Email}</p>
                        <p><b>Phone:</b> {model.Phone}</p>
                        <p><b>Message:</b> {model.Message}</p>"
                }.ToMessageBody();

                using (var smtp = new SmtpClient())
                {
                    await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync("vidhi.p.ivorytechnolab@gmail.com", "yhgyxvmsrbjhgtav");
                    await smtp.SendAsync(adminMsg);
                    await smtp.DisconnectAsync(true);
                }

                //     SEND AUTO REPLY TO USER
                var userMsg = new MimeMessage();
                userMsg.From.Add(new MailboxAddress("Titan Technology", "vidhi.p.ivorytechnolab@gmail.com"));
                userMsg.To.Add(new MailboxAddress(model.Name, model.Email));
                userMsg.Subject = "Thank you for contacting us";

                userMsg.Body = new BodyBuilder
                {
                    HtmlBody = $@"
                        <p>Dear {model.Name},</p>
                        <p>Thank you for contacting us. We received your message:</p>
                        <blockquote>{model.Message}</blockquote>
                        <p>Our team will get back to you soon.</p>
                        <br/>
                        <p>Regards,<br>Titan Technology</p>"
                }.ToMessageBody();

                using (var smtp = new SmtpClient())
                {
                    await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync("vidhi.p.ivorytechnolab@gmail.com", "yhgyxvmsrbjhgtav");
                    await smtp.SendAsync(userMsg);
                    await smtp.DisconnectAsync(true);
                }

                return Ok(new { success = true, message = "Message saved & email sent!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }
    }
}