using Internal_Portal.Interface;
using Internal_Portal.Models;
using Internal_Portal.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompany _repo;
        public CompanyController(ICompany repo) => _repo = repo;

        // GET: api/Company
        [HttpGet]
        [Authorize(Policy = "Company.View")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        // GET: api/Company/{code}
        [HttpGet("{code}")]
        [Authorize(Policy = "Company.GetByCode")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var item = await _repo.GetByCodeAsync(code);
            return item is null ? NotFound() : Ok(item);
        }

        // POST: api/Company  (Merge insert/update)
        [HttpPost]
        [Authorize(Policy = "Company.AddUpdate")]

        public async Task<IActionResult> InsertUpdate([FromBody] CompanyMaster model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var saved = await _repo.InsertUpdateAsync(model);

            // You can return CreatedAtAction if you want 201 for new insert,
            // but MERGE does not say whether insert or update => return OK
            return Ok(saved);
        }

        // DELETE: api/Company/{code}
        [HttpDelete("{code}")]
        [Authorize(Policy = "Company.Delete")]

        public async Task<IActionResult> Delete(string code)
        {
            var deleted = await _repo.DeleteAsync(code);

            if (!deleted)
                return NotFound(new { message = $"Company with code {code} not found." });

            return Ok(new { message = "Deleted successfully" });
        }

    }
}