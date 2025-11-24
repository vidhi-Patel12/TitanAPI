using System.ComponentModel.DataAnnotations;

namespace Internal_Portal.Models
{
    public class RolePermissionMaster
    {
        [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public UserRoleMaster? Role { get; set; }

        public int PermissionId { get; set; }
        public PermissionMaster? Permission { get; set; }

        public int? ModuleId { get; set; }

    }
}
