using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface IVendor
    {
        Task<IEnumerable<VendorMaster>> GetAllAsync();
        Task<VendorMaster?> GetByIdAsync(int vendorId);
        Task<VendorMaster> InsertUpdateAsync(VendorMaster vendor);
        Task<bool> DeleteAsync(int vendorId);
    }
}
