using System.ComponentModel.DataAnnotations;

namespace Internal_Portal.Models
{
    public class CompanyMaster
    {
        [Key]
        [MaxLength(20)]
        public string CompanyCode { get; set; }

        [Required]
        [MaxLength(255)]
        public string CompanyName { get; set; }

        public string CompanyAddress { get; set; }
        public string Gstn { get; set; }
        public string PanNumber { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonNumber { get; set; }
        public string PaymentTerms { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public string IfscCode { get; set; }
        public string Micr { get; set; }

        // Navigation properties
        public ICollection<VendorMaster>? Vendors { get; set; }
        public ICollection<CustomerMaster>? Customers { get; set; }
        public ICollection<EmployeeMaster>? Employees { get; set; }
    }
}
