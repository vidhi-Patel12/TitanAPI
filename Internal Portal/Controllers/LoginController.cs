using Internal_Portal.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogin _repo;

        public LoginController(ILogin repo)
        {
            _repo = repo;
        }

        // POST: api/Login/password
        [HttpPost("password")]
        public async Task<IActionResult> LoginWithPassword([FromBody] PasswordLoginRequest request)
        {
            var user = await _repo.LoginWithPasswordAsync(request.contact_number, request.Password);

            if (user == null)
                return Unauthorized(new { message = "Invalid contact number or password" });

            return Ok(new { message = "Login successful", user });
        }

        // POST: api/Login/request-otp
        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] OtpRequest request)
        {
            var user = await _repo.GetUserByContactAsync(request.contact_number);

            if (user == null)
                return NotFound(new { message = "Contact number not registered." });

            var otpEntry = await _repo.GenerateOtpAsync(user);

            // TODO: Send OTP via SMS service
            return Ok(new { message = "OTP generated successfully", otp = otpEntry.OTP });
        }

        // POST: api/Login/verify-otp
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest request)
        {
            var user = await _repo.GetUserByContactAsync(request.contact_number);
            if (user == null)
                return NotFound(new { message = "User not found." });

            var otpEntry = await _repo.GetLatestValidOtpAsync(user.Id);

            if (otpEntry == null || otpEntry.OTP != request.OTP)
                return BadRequest(new { message = "Invalid OTP" });

            if (DateTime.Now > otpEntry.GeneratedAt.AddMinutes(5))
            {
                await _repo.InvalidateOtpAsync(otpEntry);
                return BadRequest(new { message = "OTP expired. Please request a new one." });
            }

            await _repo.InvalidateOtpAsync(otpEntry);

            return Ok(new { message = "OTP verified successfully", user });
        }

        // POST: api/Login/expire-otps
        [HttpPost("expire-otps")]
        public async Task<IActionResult> ExpireOtps()
        {
            await _repo.ExpireOtpsAsync();
            return Ok(new { message = "Expired OTPs invalidated successfully" });
        }

        // DTOs
        public class PasswordLoginRequest
        {
            public string contact_number { get; set; }
            public string Password { get; set; }
        }

        public class OtpRequest
        {
            public string contact_number { get; set; }
        }

        public class OtpVerifyRequest
        {
            public string contact_number { get; set; }
            public int OTP { get; set; }
        }
    }
}