using Internal_Portal.Interface;
using Internal_Portal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Internal_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolePermissionController : ControllerBase
    {
        private readonly IUserRolePermission _repo;

        public UserRolePermissionController(IUserRolePermission repo)
        {
            _repo = repo;
        }

        [HttpGet("GetAllPermissions")]
        [Authorize(Policy = "Admin.ViewAllPermissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var result = await _repo.GetAllPermissionsAsync();
            return Ok(result);
        }

        [HttpGet] 
        [Authorize(Policy = "Admin.ViewAllRolePermissions")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repo.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Admin.ViewByIdRolePermissions")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _repo.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        //[HttpPost]
        //public async Task<IActionResult> InsertOrUpdate([FromBody] RolePermissionMaster rolePermission)
        //{
        //    var id = await _repo.InsertOrUpdateAsync(rolePermission);
        //    return Ok(new { Success = id > 0, Id = id });
        //}

        [HttpPost]
        [Authorize(Policy = "Admin.AddEditRolePermissions")]
        public async Task<IActionResult> InsertOrUpdate([FromBody] RolePermissionDto model)
        {
            if (model.PermissionIds == null || model.PermissionIds.Length == 0)
                return BadRequest("No permissions selected.");

            // If updating a single record, pass model.Id, otherwise null
            int? updateId = model.Id > 0 ? model.Id : null;

            var count = await _repo.InsertOrUpdateAsync(model.RoleId, model.PermissionIds, updateId);
            return Ok(new { Success = count > 0, Count = count });
        }



        [HttpDelete("{roleId}")]
        [Authorize(Policy = "Admin.DeleteRolePermissions")]
        public async Task<IActionResult> DeleteByRoleId(int roleId)
        {
            var success = await _repo.DeleteByRoleIdAsync(roleId);

            if (!success)
                return NotFound(new { Message = "No permissions found for this RoleId." });

            return Ok(new { Deleted = true });
        }


    }
}