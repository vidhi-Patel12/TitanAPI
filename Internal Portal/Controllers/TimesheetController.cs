using Internal_Portal.Interface;
using Internal_Portal.Models;
using Internal_Portal.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimesheetController : ControllerBase
    {
        private readonly ITimesheet _repo;
        private readonly ILogger<TimesheetController> _logger;

        public TimesheetController(ITimesheet repo, ILogger<TimesheetController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        //[Authorize(Policy = "Timesheet.View")]
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

        [HttpGet("GetByProjectCodeWorkMonth")]
        public async Task<IActionResult> GetByProjectCodeWorkMonth([FromQuery] string projectcode, [FromQuery] DateOnly workmonth, [FromQuery] int employeeid)
        {
            var result = await _repo.GetByProjectCodeWorkMonthAsync(projectcode, workmonth,employeeid);

            if (result == null || !result.Any())
                return NotFound(new { message = "Timesheet already exists." });

            return Ok(result);
        }


        [HttpGet("ByMonth")]
        public async Task<IActionResult> GetByMonth([FromQuery] int year, [FromQuery] int month)
        {
            if (year <= 0 || month <= 0 || month > 12)
                return BadRequest("Invalid year or month");

            var result = await _repo.GetByMonthAsync(year, month);
            return Ok(result);
        }



        [HttpPost("InsertUpdate")]
        public async Task<IActionResult> InsertUpdate([FromBody] List<Timesheet> timesheets)
        {
            if (timesheets == null || !timesheets.Any())
                return BadRequest("No timesheets to save.");
            try
            {
                var savedTimesheets = new List<Timesheet>();

            foreach (var ts in timesheets)
            {
                // Save each timesheet via repository
                var result = await _repo.InsertUpdateAsync(ts);
                savedTimesheets.Add(result);
            }

            return Ok(savedTimesheets);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while saving timesheets");
                return StatusCode(500, new { error = "Database error", detail = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving timesheets");
                return StatusCode(500, new { error = "Server error", detail = ex.Message });
            }
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

        [HttpGet("GetTimesheetReport")]
        //[Authorize(Policy = "AdminTimesheetReport.View")]
        public async Task<IActionResult> GetTimesheetReport(int? employeeId = null, DateTime? workMonth = null, string timesheetType = null)
        {
            var result = await _repo.GetTimesheetReportAsync(employeeId,workMonth, timesheetType);
            return Ok(result);
        }


        //[HttpPost("{id}/generate-entries")]
        //public IActionResult GenerateEntries([FromBody] Timesheet timesheet)
        //{
        //    var entries = _repo.GenerateTimesheetEntries(timesheet);
        //    return Ok(entries);
        //}
    }
}
