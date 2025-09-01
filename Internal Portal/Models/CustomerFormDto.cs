using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Internal_Portal.Models
{
    public class CustomerFormDto
    {
        [SwaggerSchema("Customer ID (0 for insert, >0 for update)")]
        public int CustomerId { get; set; }

        [Required]
        [SwaggerSchema("Company code")]
        public string CompanyCode { get; set; } = string.Empty;

        [Required]
        [SwaggerSchema("Customer name")]
        public string CustomerName { get; set; } = string.Empty;

        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? Gstn { get; set; }
        public string? PanNumber { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonNumber { get; set; }
        public string? PaymentTerms { get; set; }

        [SwaggerSchema("Agreement document 1")]
        public IFormFile? AgreementFile1 { get; set; }

        [SwaggerSchema("Agreement document 2")]
        public IFormFile? AgreementFile2 { get; set; }

        [SwaggerSchema("Agreement document 3")]
        public IFormFile? AgreementFile3 { get; set; }

        [SwaggerSchema("Agreement document 4")]
        public IFormFile? AgreementFile4 { get; set; }
    }
}