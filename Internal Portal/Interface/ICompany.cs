using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface ICompany
    {
        Task<IEnumerable<CompanyMaster>> GetAllAsync();
        Task<CompanyMaster?> GetByCodeAsync(string companyCode);
        Task<CompanyMaster> InsertUpdateAsync(CompanyMaster model); // insert or update (MERGE SP)
        Task<bool> DeleteAsync(string companyCode);
    }
}
