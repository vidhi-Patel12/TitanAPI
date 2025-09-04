using Internal_Portal.Data;
using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Internal_Portal.Repository
{
    public class LoginRepository : ILogin
    {
        private readonly AppDbContext _context;

        public LoginRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Register?> GetUserByContactAsync(string contactNumber)
        {
            return await _context.Register
                .FirstOrDefaultAsync(r => r.contact_number == contactNumber);
        }

        public async Task<Register?> LoginWithPasswordAsync(string contactNumber, string password)
        {
            return await _context.Register
         .Include(r => r.UserRole) // Include related UserRole
         .FirstOrDefaultAsync(r => r.contact_number == contactNumber && r.Password == password);
        }

        public async Task<Login> GenerateOtpAsync(Register user)
        {
            var otp = new Random().Next(100000, 999999);

            var login = new Login
            {
                id = user.Id,
                OTP = otp,
                IsValid = true,
                GeneratedAt = DateTime.Now
            };

            await _context.Login.AddAsync(login);
            await _context.SaveChangesAsync();
            return login;
        }

        public async Task<Login?> GetLatestValidOtpAsync(int registerId)
        {
            return await _context.Login
                .Where(l => l.id == registerId && l.IsValid)
                .OrderByDescending(l => l.GeneratedAt)
                .FirstOrDefaultAsync();
        }

        public async Task InvalidateOtpAsync(Login otpEntry)
        {
            otpEntry.IsValid = false;
            await _context.SaveChangesAsync();
        }

        public async Task ExpireOtpsAsync()
        {
            var expired = await _context.Login
                .Where(l => l.IsValid && DateTime.Now > l.GeneratedAt.AddMinutes(5))
                .ToListAsync();

            foreach (var otp in expired)
            {
                otp.IsValid = false;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}