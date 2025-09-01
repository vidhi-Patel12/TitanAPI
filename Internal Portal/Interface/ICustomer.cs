using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface ICustomer
    {
        Task<IEnumerable<CustomerMaster>> GetAllAsync();
        Task<CustomerMaster?> GetByIdAsync(int customerId);
        Task<CustomerMaster> InsertUpdateAsync(CustomerMaster model);
        Task<bool> DeleteAsync(int customerId);
    }
}
