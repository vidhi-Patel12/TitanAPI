using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareerController : ControllerBase
    {
        private readonly ICareer _careerRepo;
        private readonly IWebHostEnvironment _env;

        public CareerController(ICareer careerRepo, IWebHostEnvironment env)
        {
            _careerRepo = careerRepo;
            _env = env;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var career = await _careerRepo.GetCareerById(id);
            if (career == null) return NotFound();
            return Ok(career);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _careerRepo.GetAllCareers());
        }

        [HttpGet("ByName/{jobTitle}")]
        public async Task<IActionResult> GetByName(string jobTitle)
        {
            var career = await _careerRepo.GetCareerByName(jobTitle);
            if (career == null) return NotFound();
            return Ok(career);
        }


        [HttpPost("Post")]
        public async Task<IActionResult> Create([FromForm] Career career, [FromQuery] bool? updateIfExists = null)
        {
            // 1. Check if career already exists by name
            var existing = await _careerRepo.GetCareerByName(career.JobTitle);

            if (existing != null && updateIfExists == null)
            {
                // Conflict, ask user what to do
                return Conflict(new
                {
                    Message = $"Career with name '{career.JobTitle}' already exists.",
                    ExistingCareerId = existing.CareerId,
                    Options = "Pass updateIfExists=true to update, or updateIfExists=false to insert a new record."
                });
            }


            // 3. Decide whether to update or insert
            if (existing != null && updateIfExists == true)
            {
                career.CareerId = existing.CareerId;
                await _careerRepo.UpdateCareer(career);
                return Ok(new { Message = "Career updated successfully", CareerId = career.CareerId });
            }
            else
            {
                var id = await _careerRepo.CreateCareer(career);
                return Ok(new { Message = "Career created successfully", CareerId = id });
            }
        }


        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] Career career)
        {
            await _careerRepo.UpdateCareer(career);
            return Ok(new { career.CareerId });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int updatedBy)
        {
            if (updatedBy <= 0) return BadRequest(new { message = "updatedBy query parameter is required." });

            var deleted = await _careerRepo.DeleteAsync(id, updatedBy);

            if (!deleted)
                return NotFound(new { message = $"DropdownMaster with id {id} not found." });

            return Ok(new { message = "Deleted successfully" });
        }
    }
}