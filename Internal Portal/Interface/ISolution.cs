using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface ISolution
    {
        Task<int> CreateSolution(Solution solution);
        Task<Solution> GetSolutionById(int id);
        Task<IEnumerable<Solution>> GetAllSolutions();
        Task UpdateSolution(Solution solution);
        Task<bool> DeleteAsync(int id, int updatedBy);

        Task<Solution?> GetSolutionByName(string solutionName);

    }
}
