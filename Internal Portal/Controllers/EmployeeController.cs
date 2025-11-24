using Internal_Portal.Interface;
using Internal_Portal.Models;
using Internal_Portal.Repository;
using Internal_Portal.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployee _repo;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(IEmployee repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        // GET: api/Employee
        [HttpGet]
        [Authorize(Policy = "Employee.View")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        // GET: api/Employee/{id}
        [HttpGet("{id:int}")]
        [Authorize(Policy = "Employee.ViewById")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            return item is null ? NotFound(new { message = $"Employee {id} not found." }) : Ok(item);
        }

        [HttpPost("InsertUpdate")]
        [Authorize(Policy = "Employee.InsertUpdate")]
        public async Task<IActionResult> InsertUpdate([FromForm] EmployeeFormDto dto)
        {
            var employee = new EmployeeMaster
            {
                EmployeeId = dto.EmployeeId,
                EmployeeType = dto.EmployeeType,
                CompanyCode = dto.CompanyCode,
                VendorId = dto.VendorId,
                Name = dto.Name,
                AltName = dto.AltName,
                Age = dto.Age,
                SkillSet = dto.SkillSet,
                Experience = dto.Experience,
                TimingAvailability = dto.TimingAvailability,
                ContactNumber1 = dto.ContactNumber1,
                ContactNumber2 = dto.ContactNumber2,
                Remarks = dto.Remarks,
                ReferredBy = dto.ReferredBy,
                CreatedBy = dto.CreatedBy,

                // Save files
                NdaUpload = FileHelper.SaveFile(dto.NdaFile, "uploads/nda", _env),

                AadharUpload = FileHelper.SaveFile(dto.AadharFile1, "uploads/aadhar", _env),
                PanNumber = dto.PanNumber1,
                PanUpload = FileHelper.SaveFile(dto.PanFile1, "uploads/pan", _env),
                AccountNumber1 = dto.AccountNumber1,
                IfscCode1 = dto.IfscCode1,
                AccountName1 = dto.AccountName1,
                Cheque1Upload = FileHelper.SaveFile(dto.ChequeFile1, "uploads/cheque", _env),

                Aadhar2Upload = FileHelper.SaveFile(dto.AadharFile2, "uploads/aadhar", _env),
                PanNumber2 = dto.PanNumber2,
                PanUpload2 = FileHelper.SaveFile(dto.PanFile2, "uploads/pan", _env),
                AccountNumber2 = dto.AccountNumber2,
                IfscCode2 = dto.IfscCode2,
                AccountName2 = dto.AccountName2,
                Cheque2Upload = FileHelper.SaveFile(dto.ChequeFile2, "uploads/cheque", _env)
            };

            var result = await _repo.InsertUpdateAsync(employee);
            if (result == null)
                return BadRequest("Insert/Update failed");

            return Ok(result);
        }



        // DELETE: api/Employee/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Employee.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = $"Employee {id} not found." });
            return Ok(new { message = "Deleted successfully" });
        }
    }
}