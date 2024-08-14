using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataTransmissionAPI.Dto;
using DataTransmissionAPI.Service;

namespace DataTransmissionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _service;

        public RoleController(RoleService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("list")]
        public async Task<List<RoleDto>> GetAllRoles()
        {
            return await _service.GetAllRolesAsync();
        }

        [HttpGet]
        [Route("{roleId}")]
        public async Task<RoleDto> GetRoleById(string roleId)
        {
            return await _service.GetRoleByIdAsync(roleId);
        }

        [HttpPost]
        [Route("save")]
        public async Task<ActionResult> SaveRole(RoleDto model)
        {
            var res = await _service.SaveRoleAsync(model);
            if (res == true)
            {
                return Ok(new { message = "Saved role successfully" });
            }
            else
            {
                return BadRequest(new { message = "Save role failed", error = true });
            }
        }

        [HttpPost]
        [Route("detete/{roleId}")]
        public async Task<ActionResult> DeleteRole(string roleId)
        {
            var res = await _service.DeleteRoleAsync(roleId);
            if (res == true)
            {
                return Ok(new { message = "Role successfully deleted" });
            }
            else
            {
                return BadRequest(new { message = "Removing role failed", error = true });
            }
        }
    }
}
