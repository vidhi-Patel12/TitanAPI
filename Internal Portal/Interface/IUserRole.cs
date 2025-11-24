using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface IUserRole
    {
        Task<IEnumerable<UserRoleMaster>> GetAllAsync();
        Task<UserRoleMaster?> GetByIdAsync(int id);
        Task<int> InsertOrUpdateAsync(UserRoleMaster role);
        Task<bool> DeleteAsync(int id);
    }
}
