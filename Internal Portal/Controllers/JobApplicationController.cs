using Internal_Portal.Interface;
using Internal_Portal.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplication _repo;
        private readonly IConfiguration _config;

        public JobApplicationController(IJobApplication repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("Apply")]
        public async Task<IActionResult> Apply([FromForm] JobApplication model, IFormFile? ResumeFile)
        {
            if (model == null)
                return BadRequest("Invalid payload");

            try
            {
                string resumePhysicalPath = null;
                string resumePublicUrl = "https://api.titentechnology.com";

                // 1) SAVE RESUME FILE

                if (ResumeFile != null && ResumeFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/resumes");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid() + Path.GetExtension(ResumeFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ResumeFile.CopyToAsync(stream);
                    }

                    model.ResumeUrl = "/resumes/" + fileName;
                }

                // 2) SAVE IN DATABASE
                var newId = await _repo.InsertApplicationAsync(model);
                model.Id = newId;


                // 3) SEND EMAIL TO ADMIN
                string jobTitle = await _repo.GetJobTitleByIdAsync(model.CareerId);

                var adminEmail = _config["EmailSettings:AdminEmail"];

                var adminMsg = new MimeMessage();
                adminMsg.From.Add(new MailboxAddress("Careers Portal", "vidhi.p.ivorytechnolab@gmail.com"));
                adminMsg.To.Add(new MailboxAddress("Admin", adminEmail));
                adminMsg.Subject = $"New Job Application (Ref #{model.Id})";

                var adminBody = new BodyBuilder
                {
                    HtmlBody = $@"
                <h3>New Job Application Received</h3>
                <p><b>Job Title:</b> {System.Net.WebUtility.HtmlEncode(jobTitle)}</p>
                <p><b>Name:</b> {model.ApplicantName}</p>
                <p><b>Email:</b> {model.Email}</p>
                <p><b>Phone:</b> {model.Phone}</p>
                <p><b>Resume:</b>  <a href=""{resumePublicUrl}/{model.ResumeUrl}"" target=""_blank"">Download Resume</a></p>
                        <p><b>Cover Letter:</b><br/>{(string.IsNullOrWhiteSpace(model.CoverLetter) ? "(none)" : System.Net.WebUtility.HtmlEncode(model.CoverLetter).Replace("\n", "<br/>"))}</p>"
                };

                // attach resume if uploaded
                if (!string.IsNullOrEmpty(resumePhysicalPath) && System.IO.File.Exists(resumePhysicalPath))
                {
                    adminBody.Attachments.Add(resumePhysicalPath);
                }

                adminMsg.Body = adminBody.ToMessageBody();

                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync("vidhi.p.ivorytechnolab@gmail.com", "yhgyxvmsrbjhgtav");
                    await smtp.SendAsync(adminMsg);
                    await smtp.DisconnectAsync(true);
                }

                //               4) AUTO-REPLY TO APPLICANT
                var userMsg = new MimeMessage();
                userMsg.From.Add(new MailboxAddress("HR Team", "vidhi.p.ivorytechnolab@gmail.com"));
                userMsg.To.Add(new MailboxAddress(model.ApplicantName, model.Email));
                userMsg.Subject = "Thank you for applying";

                userMsg.Body = new BodyBuilder
                {
                    HtmlBody = $@"
                <p>Dear {model.ApplicantName},</p>
                 <p>Thank you for applying for the <b>{jobTitle}</b> position.</p>
                 <p>Your application reference number is <b>{model.Id}</b>.</p>
                 <p>We will review your profile and contact you soon.</p>
                 <br/>
                 <p>Regards,<br>HR Team</p>"
                }.ToMessageBody();

                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync("vidhi.p.ivorytechnolab@gmail.com", "yhgyxvmsrbjhgtav");
                    await smtp.SendAsync(userMsg);
                    await smtp.DisconnectAsync(true);
                }


                return Ok(new { success = true, message = "Application saved and emails sent!", id = model.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message, stack = ex.StackTrace });
            }
        }
    
    }
}
