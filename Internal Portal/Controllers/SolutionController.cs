using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolutionController : ControllerBase
    {
        private readonly ISolution _solutionRepo;
        private readonly IWebHostEnvironment _env;

        public SolutionController(ISolution solutionRepo, IWebHostEnvironment env)
        {
            _solutionRepo = solutionRepo;
            _env = env;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var solution = await _solutionRepo.GetSolutionById(id);
            if (solution == null) return NotFound();
            return Ok(solution);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _solutionRepo.GetAllSolutions());
        }

        [HttpGet("ByName/{solutionName}")]
        public async Task<IActionResult> GetByName(string solutionName)
        {
            var solution = await _solutionRepo.GetSolutionByName(solutionName);
            if (solution == null) return NotFound();
            return Ok(solution);
        }


        [HttpPost("Post")]
        public async Task<IActionResult> Create([FromForm] Solution solution, IFormFile imageFile, [FromQuery] bool? updateIfExists = null)
        {
            // 1. Check if solution already exists by name
            var existing = await _solutionRepo.GetSolutionByName(solution.SolutionName);

            if (existing != null && updateIfExists == null)
            {
                // Conflict, ask user what to do
                return Conflict(new
                {
                    Message = $"Solution with name '{solution.SolutionName}' already exists.",
                    ExistingSolutionId = existing.SolutionId,
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

                solution.Image = "/uploads/" + fileName;
            }

            // 3. Decide whether to update or insert
            if (existing != null && updateIfExists == true)
            {
                solution.SolutionId = existing.SolutionId;
                await _solutionRepo.UpdateSolution(solution);
                return Ok(new { Message = "Solution updated successfully", SolutionId = solution.SolutionId });
            }
            else
            {
                var id = await _solutionRepo.CreateSolution(solution);
                return Ok(new { Message = "Solution created successfully", SolutionId = id, solution.Image });
            }
        }


        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] Solution solution, IFormFile imageFile)
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
                solution.Image = "/uploads/" + fileName;
            }

            await _solutionRepo.UpdateSolution(solution);
            return Ok(new { solution.SolutionId, solution.Image });
        }



        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int updatedBy)
        {
            if (updatedBy <= 0) return BadRequest(new { message = "updatedBy query parameter is required." });

            var deleted = await _solutionRepo.DeleteAsync(id, updatedBy);

            if (!deleted)
                return NotFound(new { message = $"DropdownMaster with id {id} not found." });

            return Ok(new { message = "Deleted successfully" });
        }
    }
}