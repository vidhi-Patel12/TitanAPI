using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IService _serviceRepo;
        private readonly IWebHostEnvironment _env;

        public ServiceController(IService serviceRepo, IWebHostEnvironment env)
        {
            _serviceRepo = serviceRepo;
            _env = env;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var service = await _serviceRepo.GetServiceById(id);
            if (service == null) return NotFound();
            return Ok(service);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _serviceRepo.GetAllServices());
        }

        [HttpGet("ByName/{serviceName}")]
        public async Task<IActionResult> GetByName(string serviceName)
        {
            var service = await _serviceRepo.GetServiceByName(serviceName);
            if (service == null) return NotFound();
            return Ok(service);
        }


        [HttpPost("Post")]
        public async Task<IActionResult> Create([FromForm] Service service, IFormFile imageFile, [FromQuery] bool? updateIfExists = null)
        {
            // 1. Check if service already exists by name
            var existing = await _serviceRepo.GetServiceByName(service.ServiceName);

            if (existing != null && updateIfExists == null)
            {
                // Conflict, ask user what to do
                return Conflict(new
                {
                    Message = $"Service with name '{service.ServiceName}' already exists.",
                    ExistingServiceId = existing.ServiceId,
                    Options = "Pass updateIfExists=true to update, or updateIfExists=false to insert a new record."
                });
            }

            // 2. Handle file upload
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

                service.Image = "/uploads/" + fileName;
            }

            // 3. Decide whether to update or insert
            if (existing != null && updateIfExists == true)
            {
                service.ServiceId = existing.ServiceId;
                await _serviceRepo.UpdateService(service);
                return Ok(new { Message = "Service updated successfully", ServiceId = service.ServiceId });
            }
            else
            {
                var id = await _serviceRepo.CreateService(service);
                return Ok(new { Message = "Service created successfully", ServiceId = id, service.Image });
            }
        }


        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] Service service, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Get the original uploaded file name (safe version)
                var fileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Save relative path in DB
                service.Image = "/uploads/" + fileName;
            }

            await _serviceRepo.UpdateService(service);
            return Ok(new { service.ServiceId, service.Image });
        }



        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int updatedBy)
        {
            if (updatedBy <= 0) return BadRequest(new { message = "updatedBy query parameter is required." });

            var deleted = await _serviceRepo.DeleteAsync(id, updatedBy);

            if (!deleted)
                return NotFound(new { message = $"Service with id {id} not found." });

            return Ok(new { message = "Deleted successfully" });
        }
    }
}