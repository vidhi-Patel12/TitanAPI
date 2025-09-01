using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectMasterController : ControllerBase
    {
        private readonly IProjectMaster _repo;
        public ProjectMasterController(IProjectMaster repo) => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

        [HttpGet("{projectCode}")]
        public async Task<IActionResult> GetByCode(string projectCode)
        {
            var project = await _repo.GetByCodeAsync(projectCode);
            return project is null ? NotFound() : Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> InsertUpdate(ProjectMaster project)
        {
            if (project is null) return BadRequest();
            var result = await _repo.InsertUpdateAsync(project);
            return Ok(result);
        }

        [HttpDelete("{projectCode}")]
        public async Task<IActionResult> Delete(string projectCode)
        {
            var success = await _repo.DeleteAsync(projectCode);
            return success
                ? Ok(new { Message = "Project deleted successfully" })
                : NotFound(new { Message = $"Project with code {projectCode} not found" });
        }

    }
}