using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Internal_Portal.Models
{
    public class EmployeeMaster
    {
        [Key]
        public int EmployeeId { get; set; }

        public string EmployeeType { get; set; }

        [ForeignKey("CompanyMaster")]
        public string CompanyCode { get; set; }

        [ForeignKey("VendorMaster")]
        public int? VendorId { get; set; }

        public string Name { get; set; }
        public string AltName { get; set; }
        public int? Age { get; set; }
        public string SkillSet { get; set; }
        public decimal? Experience { get; set; }
        public string TimingAvailability { get; set; }
        public string ContactNumber1 { get; set; }
        public string ContactNumber2 { get; set; }
        public string Remarks { get; set; }
        public string ReferredBy { get; set; }
        public string CreatedBy { get; set; }
        public string NdaUpload { get; set; }

        // Account 1
        public string AadharUpload { get; set; }
        public string PanNumber { get; set; }
        public string PanUpload { get; set; }
        public string AccountNumber1 { get; set; }
        public string IfscCode1 { get; set; }
        public string AccountName1 { get; set; }
        public string Cheque1Upload { get; set; }

        // Account 2
        public string Aadhar2Upload { get; set; }
        public string PanNumber2 { get; set; }
        public string PanUpload2 { get; set; }
        public string AccountNumber2 { get; set; }
        public string IfscCode2 { get; set; }
        public string AccountName2 { get; set; }
        public string Cheque2Upload { get; set; }

        public CompanyMaster CompanyMaster { get; set; }
        public VendorMaster VendorMaster { get; set; }
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
        public ICollection<Timesheet> Timesheets { get; set; }
    }


}
