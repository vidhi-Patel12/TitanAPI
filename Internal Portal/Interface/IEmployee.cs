using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface IEmployee
    {
        Task<IEnumerable<EmployeeMaster>> GetAllAsync();
        Task<EmployeeMaster?> GetByIdAsync(int employeeId);
        Task<EmployeeMaster> InsertUpdateAsync(EmployeeMaster model);
        Task<bool> DeleteAsync(int employeeId);
    }
}
