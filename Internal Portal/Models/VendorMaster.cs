using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Internal_Portal.Models
{
    public class VendorMaster
    {
        [Key]
        public int VendorId { get; set; }

        [ForeignKey("CompanyMaster")]
        public string CompanyCode { get; set; }
        public string VendorName { get; set; }
        public string Address { get; set; }
        public string Gstn { get; set; }
        public string PanNumber { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonNumber { get; set; }
        public string PaymentTerms { get; set; }
        public string? GstnUpload { get; set; }
        public string? PanUpload { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountNumber1 { get; set; }
        public string IfscCode { get; set; }
        public string BankAccountName { get; set; }
        public string? CancelledCheque { get; set; }
        public string? Agreement1 { get; set; }
        public string? Agreement2 { get; set; }
        public string? Agreement3 { get; set; }
        public string? Agreement4 { get; set; }

        public CompanyMaster CompanyMaster { get; set; }
        public ICollection<EmployeeMaster>? Employees { get; set; } = new List<EmployeeMaster>();
    }
}
