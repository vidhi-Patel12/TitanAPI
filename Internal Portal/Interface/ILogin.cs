using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface ILogin
    {
        Task<Register?> GetUserByContactAsync(string contactNumber);
        Task<Register?> LoginWithPasswordAsync(string contactNumber, string password);

        Task<Login> GenerateOtpAsync(Register user);
        Task<Login?> GetLatestValidOtpAsync(int registerId);
        Task InvalidateOtpAsync(Login otpEntry);
        Task ExpireOtpsAsync();

        Task SaveChangesAsync();
    }
}
