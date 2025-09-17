using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface IDropdownMaster
    {
        Task<IEnumerable<DropdownMaster>> GetAllAsync();
        Task<DropdownMaster?> GetByIdAsync(int id);
        Task<IEnumerable<DropdownMaster?>> GetByNameAsync(string name);
        Task<DropdownMaster> InsertUpdateAsync(DropdownMaster model);

        Task<bool> DeleteAsync(int id, int updatedBy);
    }
}
