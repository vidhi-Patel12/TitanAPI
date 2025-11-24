using Internal_Portal.Interface;
using Internal_Portal.Models;
using Internal_Portal.Repository;
using Internal_Portal.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomer _repo;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CustomerController> _logger;

        // Allowed file extensions and max size (adjust to your policy)
        private static readonly HashSet<string> AllowedExtensions = new HashSet<string>
        {
            ".pdf", ".doc", ".docx", ".png", ".jpg", ".jpeg", ".xlsx", ".xls", ".txt"
        };
        private const long MaxFileBytes = 20 * 1024 * 1024; // 20 MB per file

        public CustomerController(ICustomer repo, IWebHostEnvironment env, ILogger<CustomerController> logger)
        {
            _repo = repo;
            _env = env;
            _logger = logger;
        }

        // GET: api/Customer
        [HttpGet]
        [Authorize(Policy = "Customer.View")]

        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        // GET: api/Customer/{id}
        [HttpGet("{id:int}")]
        [Authorize(Policy = "Customer.ViewById")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            return item is null ? NotFound(new { message = $"Customer {id} not found" }) : Ok(item);
        }

        // POST: api/Customer (Insert/Update)
        [HttpPost]
        [Authorize(Policy = "Customer.InsertUpdate")]
        [RequestSizeLimit(100_000_000)] // overall request size limit (adjust)
        [Consumes("multipart/form-data")] // Tells Swagger it's file upload
        [SwaggerOperation(
            Summary = "Insert or update a customer",
            Description = "Saves customer details with up to 4 agreement files"
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertUpdate([FromForm] CustomerFormDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var model = new CustomerMaster
            {
                CustomerId = dto.CustomerId,
                CompanyCode = dto.CompanyCode,
                CustomerName = dto.CustomerName,
                Address = dto.Address,
                Country = dto.Country,
                Gstn = dto.Gstn,
                PanNumber = dto.PanNumber,
                ContactPersonName = dto.ContactPersonName,
                ContactPersonNumber = dto.ContactPersonNumber,
                PaymentTerms = dto.PaymentTerms
            };

            // Root for agreements: wwwroot/agreements
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadRoot = Path.Combine(webRoot, "agreements");
            if (!Directory.Exists(uploadRoot)) Directory.CreateDirectory(uploadRoot);

            var newFiles = new List<string>();
            var oldFiles = new List<string>();
            CustomerMaster? existing = null;

            if (model.CustomerId > 0)
                existing = await _repo.GetByIdAsync(model.CustomerId);

            async Task<string> SaveFile(IFormFile file)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(ext)) throw new InvalidOperationException($"Extension {ext} not allowed.");
                if (file.Length > MaxFileBytes) throw new InvalidOperationException("File too large.");

                var safeName = FileNameHelper.SanitizeFileName(Path.GetFileNameWithoutExtension(file.FileName));
                var newName = $"{safeName}_{Guid.NewGuid()}{ext}";
                var abs = Path.Combine(uploadRoot, newName);

                await using (var fs = new FileStream(abs, FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }

                newFiles.Add(abs);
                return "/agreements/" + newName; // relative path for DB
            }

            try
            {
                if (dto.AgreementFile1 != null)
                {
                    model.Agreement1 = await SaveFile(dto.AgreementFile1);
                    if (!string.IsNullOrEmpty(existing?.Agreement1)) oldFiles.Add(existing.Agreement1);
                }
                if (dto.AgreementFile2 != null)
                {
                    model.Agreement2 = await SaveFile(dto.AgreementFile2);
                    if (!string.IsNullOrEmpty(existing?.Agreement2)) oldFiles.Add(existing.Agreement2);
                }
                if (dto.AgreementFile3 != null)
                {
                    model.Agreement3 = await SaveFile(dto.AgreementFile3);
                    if (!string.IsNullOrEmpty(existing?.Agreement3)) oldFiles.Add(existing.Agreement3);
                }
                if (dto.AgreementFile4 != null)
                {
                    model.Agreement4 = await SaveFile(dto.AgreementFile4);
                    if (!string.IsNullOrEmpty(existing?.Agreement4)) oldFiles.Add(existing.Agreement4);
                }

                var saved = await _repo.InsertUpdateAsync(model);

                // Delete old files after DB success
                foreach (var rel in oldFiles)
                {
                    try
                    {
                        var abs = Path.Combine(uploadRoot, Path.GetFileName(rel));
                        if (System.IO.File.Exists(abs)) System.IO.File.Delete(abs);
                    }
                    catch (Exception e) { _logger.LogWarning(e, "Failed to delete old file"); }
                }

                return Ok(saved);
            }
            catch (Exception ex)
            {
                // Rollback newly uploaded files
                foreach (var f in newFiles)
                {
                    try { if (System.IO.File.Exists(f)) System.IO.File.Delete(f); } catch { }
                }

                _logger.LogError(ex, "InsertUpdate failed");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/Customer/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Customer.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            // get existing to delete files after DB deletion
            var existing = await _repo.GetByIdAsync(id);

            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = $"Customer {id} not found" });
             // delete files (if any)
            try
            {
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadRoot = Path.Combine(webRoot, "agreements");
                var toDelete = new List<string?> { existing?.Agreement1, existing?.Agreement2, existing?.Agreement3, existing?.Agreement4 };
                foreach (var rel in toDelete.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    var filename = Path.GetFileName(rel!);
                    var abs = Path.Combine(uploadRoot, filename);
                    var full = Path.GetFullPath(abs);
                    if (full.StartsWith(Path.GetFullPath(uploadRoot)) && System.IO.File.Exists(full))
                    {
                        System.IO.File.Delete(full);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete some agreement files for customer {CustomerId}", id);
            }

            return Ok(new { message = "Deleted successfully" });
        }
    }
}