using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface IProjectMaster
    {
        Task<IEnumerable<ProjectMaster>> GetAllAsync();
        Task<ProjectMaster?> GetByCodeAsync(string projectCode);
        Task<ProjectMaster> InsertUpdateAsync(ProjectMaster project);
        Task<bool> DeleteAsync(string projectCode);

    }
}
