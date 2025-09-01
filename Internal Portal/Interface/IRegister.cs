using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface IRegister
    {
        Task<IEnumerable<Register>> GetAllAsync();
        Task<Register?> GetByIdAsync(int id);
        Task<int> InsertAsync(Register register);
        Task<bool> UpdateAsync(Register register);  
        Task<bool> DeleteAsync(int id);
    }
}
