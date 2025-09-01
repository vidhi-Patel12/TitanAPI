using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimesheetController : ControllerBase
    {
        private readonly ITimesheet _repo;
        public TimesheetController(ITimesheet repo) => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repo.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _repo.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> InsertUpdate([FromBody] Timesheet m)
        {
            var result = await _repo.InsertUpdateAsync(m);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repo.DeleteAsync(id);
            if (!success) return NotFound(new { Message = "Timesheet not found" });
            return Ok(new { Message = "Timesheet deleted successfully" });
        }

        [HttpPost("{id}/submit")]
        public async Task<IActionResult> Submit(int id, [FromQuery] string status)
        {
            var result = await _repo.SubmitAsync(id, status);
            return Ok(result);
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetReport([FromQuery] string? projectCode, [FromQuery] int? employeeId, [FromQuery] string? status)
        {
            var report = await _repo.GetReportAsync(projectCode, employeeId, status);
            return Ok(report);
        }

        [HttpPost("{id}/generate-entries")]
        public IActionResult GenerateEntries([FromBody] Timesheet timesheet)
        {
            var entries = _repo.GenerateTimesheetEntries(timesheet);
            return Ok(entries);
        }
    }
}
