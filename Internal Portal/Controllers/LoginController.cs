using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Twilio.TwiML.Voice;
using static System.Net.WebRequestMethods;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogin _repo;
        private readonly AppDbContext _db;
        private readonly SmsService _smsService;
        private readonly IConfiguration _config;

        public LoginController(ILogin repo, AppDbContext db, SmsService smsService, IConfiguration config)
        {
            _repo = repo;
            _db = db;
            _smsService = smsService;
            _config = config;

        }

        // POST: api/Login/password
        [HttpPost("password")]
        public async Task<IActionResult> LoginWithPassword([FromBody] PasswordLoginRequest request)
        {
            var user = await _repo.LoginWithPasswordAsync(request.contact_number, request.Password);

            if (user == null)
                return Unauthorized(new { message = "Invalid contact number or password" });

            // Get permissions from role
            var permissions = await (from rp in _db.RolePermissionMaster
                                     join p in _db.PermissionMaster on rp.PermissionId equals p.Id
                                     where rp.RoleId == user.UserRole.Id
                                     select p.PermissionName).ToListAsync();

            // Create claims
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.contact_number),
            new Claim("UserId", user.Id.ToString())
        };

            claims.AddRange(permissions.Select(p => new Claim("Permission", p)));

            //var claimsIdentity = new ClaimsIdentity(claims, "MyCookieScheme");

            //await HttpContext.SignInAsync("MyCookieScheme", new ClaimsPrincipal(claimsIdentity));

            //await HttpContext.SignInAsync("MyCookieScheme", new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
            //{
            //    IsPersistent = true,
            //    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            //});


            // ✅ Generate JWT token
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpireHours"])),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                user = new
                {
                    id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    contact_number = user.contact_number,
                    userRole = user.UserRole?.RoleName,
                    userRoleId = user.UserRole?.Id
                },
                permissions = permissions ,// optional: send back permissions list
                token = tokenString
                //token = await HttpContext.GetTokenAsync("MyCookieScheme")
            });

        }

        // POST: api/Login/request-otp
        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] OtpRequest request)
        {
            var user = await _repo.GetUserByContactAsync(request.contact_number);

            if (user == null)
                return NotFound(new { message = "Contact number not registered." });

            var otpEntry = await _repo.GenerateOtpAsync(user);

            bool otpSent = _smsService.SendSmsOTP(Convert.ToInt64(request.contact_number), otpEntry.OTP);

            if (!otpSent)
            {
                return StatusCode(500, new { message = "Failed to send OTP. Please try again." });
            }

            // TODO: Send OTP via SMS service
            return Ok(new { message = "OTP generated successfully", otp = otpEntry.OTP });
        }

        // POST: api/Login/verify-otp
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest request)
        {
            var user = await _repo.GetUserByOTPContactAsync(request.contact_number);
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

            if (user.UserRole == null)
            {
                return BadRequest(new { message = "User role is missing for this user." });
            }

            var roleId = user.UserRole.Id;


            // GET ROLE PERMISSIONS (same as password login)
            // ================================
            var permissions = await (from rp in _db.RolePermissionMaster
                                     join p in _db.PermissionMaster on rp.PermissionId equals p.Id
                                     where rp.RoleId == user.UserRole.Id
                                     select p.PermissionName).ToListAsync();


            // ================================
            // CREATE TOKEN (same as password login)
            // ================================
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.contact_number),
        new Claim("UserId", user.Id.ToString())
    };

            claims.AddRange(permissions.Select(p => new Claim("Permission", p)));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpireHours"])),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);


            // ================================
            // RETURN SAME STRUCTURE AS PASSWORD LOGIN
            // ================================
            return Ok(new
            {
                user = new
                {
                    id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    contact_number = user.contact_number,
                    userRole = user.UserRole?.RoleName,
                    userRoleId = user.UserRole?.Id
                },
                permissions = permissions,
                token = tokenString
            });
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