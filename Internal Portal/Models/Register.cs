using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Internal_Portal.Models
{
    public class Register
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [Column("first_name")]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("last_name")]
        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [Required, MaxLength(150)]
        [Column("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        [Column("password")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty; // store hashed password

        [MaxLength(10)]
        [Column("contact_number")]
        [JsonPropertyName("contact_number")]
        public string? contact_number { get; set; }

        [Required]
        [Column("user_role_id")]
        [JsonPropertyName("userRoleId")]

        public int UserRoleId { get; set; }

        [ForeignKey("UserRoleId")]
        [JsonIgnore]
        public virtual UserRoleMaster? UserRole { get; set; }

        [NotMapped]
        [JsonPropertyName("userRole")]
        public string? UserRoleName => UserRole?.RoleName;

    }
}
