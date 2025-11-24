using Internal_Portal.Models;

namespace Internal_Portal.Interface
{
    public interface IUserRolePermission
    {
        //Task<int> InsertOrUpdateAsync(RolePermissionMaster rolePermission);
        Task<int> InsertOrUpdateAsync(int roleId, int[] permissionIds, int? updateId = null);

        Task<bool> DeleteByRoleIdAsync(int roleId);
        Task<IEnumerable<RolePermissionMaster>> GetAllAsync();
        Task<IEnumerable<PermissionMaster>> GetAllPermissionsAsync();

        Task<RolePermissionMaster?> GetByIdAsync(int id);
        //Task<List<RolePermissionMaster>> GetByRoleIdAsync(int roleId);


    }
}
