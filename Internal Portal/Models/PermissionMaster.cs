using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Internal_Portal.Models
{
    public class PermissionMaster
    {
        [Key] public int Id { get; set; }
        public string PermissionName { get; set; } = null!;
        public int ModuleId { get; set; }
        public UserRoleMaster? Module { get; set; }

        [JsonIgnore]
        public ICollection<RolePermissionMaster>? RolePermissions { get; set; } = new List<RolePermissionMaster>();
    }

}
