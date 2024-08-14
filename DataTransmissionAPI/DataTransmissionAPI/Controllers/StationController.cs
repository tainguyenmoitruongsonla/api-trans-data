using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataTransmissionAPI.Data;
using DataTransmissionAPI.Dto;
using DataTransmissionAPI.Service;

namespace DataTransmissionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class StationController : ControllerBase
    {
        private readonly StationService _service;

        public StationController(StationService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("list")]
        public async Task<List<StationDto>> GetAllData([FromQuery] FormFilterStation filter)
        {
            return await _service.GetAllAsync(filter);
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<StationDto> GetOneData(int Id)
        {
            return await _service.GetByIdAsync(Id);
        }

        [HttpPost]
        [Route("save")]
        public async Task<ActionResult<Station>> Save(StationDto dto)
        {
            var res = await _service.SaveAsync(dto);
            if (res > 0)
            {
                return Ok(new { message = "Saved station successfully", id = res });
            }
            else
            {
                return BadRequest(new { message = "Saving station failed", error = true });
            }
        }

        [HttpGet]
        [Route("delete/{Id}")]
        public async Task<ActionResult<Station>> Delete(int Id)
        {
            var res = await _service.DeleteAsync(Id);
            if (res == true)
            {
                return Ok(new { message = "Station successfully deleted" });
            }
            else
            {
                return BadRequest(new { message = "Removing Station failed", error = true });
            }
        }
    }
}
