using Internal_Portal.Interface;
using Internal_Portal.Models;
using Internal_Portal.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IVendor _vendorRepo;
        private readonly IWebHostEnvironment _env;

        public VendorController(IVendor vendorRepo, IWebHostEnvironment env)
        {
            _vendorRepo = vendorRepo;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _vendorRepo.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var vendor = await _vendorRepo.GetByIdAsync(id);
            if (vendor == null) return NotFound();
            return Ok(vendor);
        }

        [HttpPost]
        public async Task<IActionResult> InsertUpdate([FromForm] VendorDto dto)
        {
            var vendor = new VendorMaster
            {
                VendorId = dto.VendorId,
                CompanyCode = dto.CompanyCode,
                VendorName = dto.VendorName,
                Address = dto.Address,
                Gstn = dto.Gstn,
                PanNumber = dto.PanNumber,
                ContactPersonName = dto.ContactPersonName,
                ContactPersonNumber = dto.ContactPersonNumber,
                PaymentTerms = dto.PaymentTerms,
                AccountHolderName = dto.AccountHolderName,
                AccountNumber1 = dto.AccountNumber1,
                IfscCode = dto.IfscCode,
                BankAccountName = dto.BankAccountName,

                // File uploads
                GstnUpload = FileHelper.SaveFile(dto.GstnUpload, "uploads/vendors", _env),
                PanUpload = FileHelper.SaveFile(dto.PanUpload, "uploads/vendors", _env),
                CancelledCheque = FileHelper.SaveFile(dto.CancelledCheque, "uploads/vendors", _env),
                Agreement1 = FileHelper.SaveFile(dto.Agreement1, "uploads/vendors", _env),
                Agreement2 = FileHelper.SaveFile(dto.Agreement2, "uploads/vendors", _env),
                Agreement3 = FileHelper.SaveFile(dto.Agreement3, "uploads/vendors", _env),
                Agreement4 = FileHelper.SaveFile(dto.Agreement4, "uploads/vendors", _env)
            };

            var saved = await _vendorRepo.InsertUpdateAsync(vendor);
            return Ok(saved);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _vendorRepo.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok(new { message = "Deleted successfully" });
        }
    }
}