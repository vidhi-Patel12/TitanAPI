using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface IJobApplication
    {
        Task<int> InsertApplicationAsync(JobApplication application);

        Task<string> GetJobTitleByIdAsync(int careerId);
    }
}
