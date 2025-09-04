using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Internal_Portal.Models
{
    public class UserRoleMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]

        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("role_name")]
        public string RoleName { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual ICollection<Register>? Registers { get; set; }


    }
}
