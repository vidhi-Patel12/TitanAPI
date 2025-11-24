using Microsoft.AspNetCore.Authorization;

namespace Internal_Portal.Auth
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string PermissionName { get; }
        public PermissionRequirement(string permissionName) => PermissionName = permissionName;
    }
}
