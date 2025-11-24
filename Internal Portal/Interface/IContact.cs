using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface IContact
    {
        Task<int> InsertMessage(ContactMessage message);
    }
}
