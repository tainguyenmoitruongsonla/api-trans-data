using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataTransmissionAPI.Data;
using DataTransmissionAPI.Dto;
using DataTransmissionAPI.Service;

namespace DataTransmissionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class UserDashboardController : ControllerBase
    {
        private readonly UserDashboardService _service;

        public UserDashboardController(UserDashboardService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("list")]
        public async Task<List<UserDashboardDto>> GetAllUserDashboard()
        {
            return (await _service.GetAllUserDashboardAsync());
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<UserDashboardDto> GetUserDashboardById(int Id)
        {
            return await _service.GetUserDashboardByIdAsync(Id);
        }

        [HttpPost]
        [Route("save")]
        public async Task<ActionResult<UserDashboards>> SaveUserDashboard(UserDashboardDto dto)
        {
            var res = await _service.SaveUserDashboardAsync(dto);
            if (res == true)
            {
                return Ok(new { message = "Saved user-dashboard successfully" });
            }
            else
            {
                return BadRequest(new { message = "Save user-dashboard failed", });
            }
        }

        [HttpPost]
        [Route("delete")]
        public async Task<ActionResult<UserDashboards>> DeleteUserDashboard(UserDashboardDto dto)
        {
            var res = await _service.DeleteUserDashboardAsync(dto);
            if (res == true)
            {
                return Ok(new { message = "User-dashboard successfully deleted" });
            }
            else
            {
                return BadRequest(new { message = "Removing user-dashboards failed", error = true });
            }
        }
    }
}
