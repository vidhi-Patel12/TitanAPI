using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimesheetEntryController : ControllerBase
    {
        private readonly ITimesheetEntry _repo;
        public TimesheetEntryController(ITimesheetEntry repo) => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

        [HttpGet("ByTimesheet/{timesheetId}")]
        public async Task<IActionResult> GetByTimesheet(int timesheetId) => Ok(await _repo.GetByTimesheetIdAsync(timesheetId));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var entry = await _repo.GetByIdAsync(id);
            return entry != null ? Ok(entry) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TimesheetEntry entry)
        {
            var saved = await _repo.InsertUpdateAsync(entry);
            return Ok(saved);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            return deleted ? Ok(new { Message = "Deleted successfully" }) : NotFound();
        }
    }
}