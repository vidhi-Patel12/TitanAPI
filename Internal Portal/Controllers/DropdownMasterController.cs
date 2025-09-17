using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DropdownMasterController : ControllerBase
    {
        private readonly IDropdownMaster _repo;
        public DropdownMasterController(IDropdownMaster repo) => _repo = repo;

        // GET: api/DropdownMaster
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        // GET: api/DropdownMaster/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        // GET: api/DropdownMaster/{name}
        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var items = await _repo.GetByNameAsync(name);
            if (items == null || !items.Any())
                return NotFound(new { message = $"Dropdown items with Name '{name}' not found." });

            return Ok(items);
        }



        // POST: api/DropdownMaster?userId=1
        // Insert/Update handled by the same SP
        [HttpPost]
        public async Task<IActionResult> InsertUpdate([FromBody] DropdownMaster model)
        {

            var saved = await _repo.InsertUpdateAsync(model);

            // Similar to CompanyController: return OK (insert/update merged)
            return Ok(saved);
        }

        // DELETE: api/DropdownMaster/{id}?updatedBy=1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int updatedBy)
        {
            if (updatedBy <= 0) return BadRequest(new { message = "updatedBy query parameter is required." });

            var deleted = await _repo.DeleteAsync(id, updatedBy);

            if (!deleted)
                return NotFound(new { message = $"DropdownMaster with id {id} not found." });

            return Ok(new { message = "Deleted successfully" });
        }
    }
}
