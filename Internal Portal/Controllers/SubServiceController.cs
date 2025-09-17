using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubServiceController : ControllerBase
    {
        private readonly ISubService _subServiceRepo;
        private readonly IWebHostEnvironment _env;

        public SubServiceController(ISubService subServiceRepo, IWebHostEnvironment env)
        {
            _subServiceRepo = subServiceRepo;
            _env = env;

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var subService = await _subServiceRepo.GetSubServiceById(id);
            if (subService == null) return NotFound();
            return Ok(subService);
        }

        [HttpGet("ByName/{subServiceName}")]
        public async Task<IActionResult> GetByName(string subServiceName)
        {
            var subService = await _subServiceRepo.GetSubServiceByName(subServiceName);
            if (subService == null) return NotFound();
            return Ok(subService);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _subServiceRepo.GetAllSubServices());
        }

        [HttpPost("Post")]
        public async Task<IActionResult> Create(
     [FromForm] SubService subService,
     IFormFile? imageFile,
     [FromQuery] bool? updateIfExists = null)
        {
            // 1. Check if SubService already exists by name
            var existing = await _subServiceRepo.GetSubServiceByName(subService.SubServiceName);

            if (existing != null && updateIfExists == null)
            {
                // Conflict: ask user what to do
                return Conflict(new
                {
                    Message = $"SubService with name '{subService.SubServiceName}' already exists.",
                    ExistingSubServiceId = existing.SubServiceId,
                    Options = "Pass updateIfExists=true to update, or updateIfExists=false to insert a new record."
                });
            }

            // 2. Handle file upload (if image provided)
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                subService.Image = "/uploads/" + fileName;
            }

            // 3. Decide whether to update or insert
            if (existing != null && updateIfExists == true)
            {
                subService.SubServiceId = existing.SubServiceId;
                await _subServiceRepo.UpdateSubService(subService);
                return Ok(new { Message = "SubService updated successfully", SubServiceId = subService.SubServiceId });
            }
            else
            {
                var id = await _subServiceRepo.CreateSubService(subService);
                return Ok(new { Message = "SubService created successfully", SubServiceId = id, subService.Image });
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] SubService subService, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Keep original uploaded filename
                var fileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Save relative path in DB
                subService.Image = "/uploads/" + fileName;
            }

            await _subServiceRepo.UpdateSubService(subService);
            return Ok(new { subService.SubServiceId, subService.Image });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _subServiceRepo.DeleteSubService(id);
            return Ok();
        }
    }
}