using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Internal_Portal.Models
{
    public class Login
    {
        [Key]
        public int LoginId { get; set; }
        [ForeignKey("Register")]
        [Column("id")]
        public int id { get; set; }
        public int OTP { get; set; }
        public bool IsValid { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
