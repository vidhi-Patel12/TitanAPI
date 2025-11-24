using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegister _repo;
        public RegisterController(IRegister repo) => _repo = repo;

        // GET: api/Register
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        // GET: api/Register/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        // POST: api/Register
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] Register model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = await _repo.InsertAsync(model);
            return Ok(new
            {
                success = true,
                message = "Registration successful!",
                data = model
            });
        }

        // PUT: api/Register/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Register model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != model.Id) return BadRequest("Mismatched Id");

            var updated = await _repo.UpdateAsync(model);
            if (!updated) return NotFound(new { message = $"Register with Id {id} not found." });

            return Ok(new { message = "Updated successfully" });
        }

        // DELETE: api/Register/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = $"Register with Id {id} not found." });

            return Ok(new { message = "Deleted successfully" });
        }
    }
}