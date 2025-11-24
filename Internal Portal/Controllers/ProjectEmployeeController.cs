using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectEmployeeController : ControllerBase
    {
        private readonly IProjectEmployee _repo;
        public ProjectEmployeeController(IProjectEmployee repo) => _repo = repo;

        [HttpGet]
        [Authorize(Policy = "ProjectEmployee.View")]
        public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

        [HttpGet("{id}")]
        [Authorize(Policy = "ProjectEmployee.ViewById")]
        public async Task<IActionResult> GetById(int id)
        {
            var emp = await _repo.GetByIdAsync(id);
            return emp == null ? NotFound() : Ok(emp);
        }

        [HttpGet("by-project/{projectcode}")]
        [Authorize(Policy = "ProjectEmployee.GetByProjectCode")]
        public async Task<IActionResult> GetByProjectCode(string projectcode)
        {
            var emp = await _repo.GetByProjectCodeAsync(projectcode);
            return emp == null ? NotFound() : Ok(emp);
        }

        [HttpPost]
        [Authorize(Policy = "ProjectEmployee.InsertUpdate")]
        public async Task<IActionResult> InsertUpdate(ProjectEmployee emp)
        {
            var result = await _repo.InsertUpdateAsync(emp);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ProjectEmployee.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            return deleted ? Ok(new { Message = "Deleted successfully" }) : NotFound();
        }
    }
}