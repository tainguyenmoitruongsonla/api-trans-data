using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DataTransmissionAPI.Dto;
using DataTransmissionAPI.Service;

namespace DataTransmissionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("list")]
        public async Task<List<UserDto>> GetAllUsers([FromQuery] FormFilterUser filter)
        {
            return await _service.GetAllUsersAsync(filter);
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<UserDto> GetUserById(string userId)
        {
            return await _service.GetUserByIdAsync(userId);
        }

        [HttpGet]
        [Route("getuserinfo/{userId}")]
        public async Task<UserInfoDto> GetUserInfoByIdAsync(string userId)
        {
            return await _service.GetUserInfoByIdAsync(userId);
        }

        [HttpPost]
        [Route("save")]
        public async Task<ActionResult> SaveUser(UserDto dto)
        {
            var res = await _service.SaveUserAsync(dto);
            if (res == true)
            {
                return Ok(new { message = "Saved user successfully" });
            }
            else
            {
                return BadRequest(new { message = "Save user failed", error = true });
            }
        }

        [HttpPost]
        [Route("delete")]
        public async Task<ActionResult> DeleteUser(UserDto dto)
        {
            var res = await _service.DeleteUserAsync(dto);
            if (res == true)
            {
                return Ok(new { message = "User successfully deleted" });
            }
            else
            {
                return BadRequest(new { message = "Removing user failed", error = true });
            }
        }
    }
}