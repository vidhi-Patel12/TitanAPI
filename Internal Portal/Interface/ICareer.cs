using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface ICareer
    {
        Task<int> CreateCareer(Career career);
        Task<Career> GetCareerById(int id);
        Task<IEnumerable<Career>> GetAllCareers();
        Task UpdateCareer(Career career);
        Task<bool> DeleteAsync(int id, int updatedBy);

        Task<Career?> GetCareerByName(string careerName);

    }
}
