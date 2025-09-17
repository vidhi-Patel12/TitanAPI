using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface IService
    {
        Task<int> CreateService(Service service);
        Task<Service> GetServiceById(int id);
        Task<IEnumerable<Service>> GetAllServices();
        Task UpdateService(Service service);
        Task<bool> DeleteAsync(int id, int updatedBy);
        Task<Service?> GetServiceByName(string serviceName);

    }
}
