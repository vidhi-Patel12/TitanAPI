using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Internal_Portal.Models
{
    public class SubService
    {
        [Key]
        public int SubServiceId { get; set; }
        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public string SubServiceName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}
