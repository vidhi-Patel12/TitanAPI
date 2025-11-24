namespace Internal_Portal.Models
{
    public class RolePermissionDto
    {
        public int Id { get; set; } // optional, for update

        public int RoleId { get; set; }
        public int[] PermissionIds { get; set; }
    }
}
