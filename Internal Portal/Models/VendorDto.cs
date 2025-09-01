namespace Internal_Portal.Models
{
    public class VendorDto
    {
        public int VendorId { get; set; }
        public string CompanyCode { get; set; }
        public string VendorName { get; set; }
        public string Address { get; set; }
        public string Gstn { get; set; }
        public string PanNumber { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonNumber { get; set; }
        public string PaymentTerms { get; set; }

        // File Uploads
        public IFormFile? GstnUpload { get; set; }
        public IFormFile? PanUpload { get; set; }
        public IFormFile? CancelledCheque { get; set; }
        public IFormFile? Agreement1 { get; set; }
        public IFormFile? Agreement2 { get; set; }
        public IFormFile? Agreement3 { get; set; }
        public IFormFile? Agreement4 { get; set; }

        // Bank Details
        public string AccountHolderName { get; set; }
        public string AccountNumber1 { get; set; }
        public string IfscCode { get; set; }
        public string BankAccountName { get; set; }
    }
}
