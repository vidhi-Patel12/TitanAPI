using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface IProjectEmployee
    {
        Task<IEnumerable<ProjectEmployee>> GetAllAsync();
        Task<ProjectEmployee?> GetByIdAsync(int id);
        Task<IEnumerable<ProjectEmployee>> GetByProjectCodeAsync(string projectCode);

        Task<ProjectEmployee> InsertUpdateAsync(ProjectEmployee employee);
        Task<bool> DeleteAsync(int id);

    }
}
