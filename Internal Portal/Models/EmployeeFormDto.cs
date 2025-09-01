using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Internal_Portal.Models
{
    public class EmployeeFormDto
    {
        [SwaggerSchema("Employee ID (0 for insert, >0 for update)")]
        public int EmployeeId { get; set; }

        [Required]
        [SwaggerSchema("Employee type (C2H, Freelancing, Permanent, Client Payroll, Commission)")]
        public string EmployeeType { get; set; } = string.Empty;

        [Required]
        [SwaggerSchema("Company code")]
        public string CompanyCode { get; set; } = string.Empty;

        [SwaggerSchema("Vendor Id (if applicable)")]
        public int? VendorId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? AltName { get; set; }
        public int? Age { get; set; }
        public string? SkillSet { get; set; }
        public decimal? Experience { get; set; }
        public string? TimingAvailability { get; set; }
        public string? ContactNumber1 { get; set; }
        public string? ContactNumber2 { get; set; }
        public string? Remarks { get; set; }
        public string? ReferredBy { get; set; }
        public string? CreatedBy { get; set; }

        // File Uploads
        [SwaggerSchema("Upload NDA document")]
        public IFormFile? NdaFile { get; set; }

        // Account 1
        [SwaggerSchema("Upload Aadhar card (Account 1)")]
        public IFormFile? AadharFile1 { get; set; }

        [SwaggerSchema("PAN card number (Account 1)")]
        public string? PanNumber1 { get; set; }

        [SwaggerSchema("Upload PAN card (Account 1)")]
        public IFormFile? PanFile1 { get; set; }

        public string? AccountNumber1 { get; set; }
        public string? IfscCode1 { get; set; }
        public string? AccountName1 { get; set; }

        [SwaggerSchema("Upload cancelled cheque (Account 1)")]
        public IFormFile? ChequeFile1 { get; set; }

        // Account 2
        [SwaggerSchema("Upload Aadhar card (Account 2)")]
        public IFormFile? AadharFile2 { get; set; }

        [SwaggerSchema("PAN card number (Account 2)")]
        public string? PanNumber2 { get; set; }

        [SwaggerSchema("Upload PAN card (Account 2)")]
        public IFormFile? PanFile2 { get; set; }

        public string? AccountNumber2 { get; set; }
        public string? IfscCode2 { get; set; }
        public string? AccountName2 { get; set; }

        [SwaggerSchema("Upload cancelled cheque (Account 2)")]
        public IFormFile? ChequeFile2 { get; set; }
    }
}
