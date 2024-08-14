using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataTransmissionAPI.Dto.Files;

namespace DataTransmissionAPI.Controllers
{
    [ApiController]
    [Route("file")]

    public class FileController : ControllerBase
    {
        private readonly string _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "FileUploads");

        public FileController()
        {
            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadFile([FromForm] UploadModel uploadFile)
        {
            try
            {
                if (uploadFile.File != null && uploadFile.File.Length > 0 && !string.IsNullOrEmpty(uploadFile.FilePath))
                {
                    var fileDirectory = Path.Combine(_uploadDirectory, uploadFile.FilePath);
                    if (!Directory.Exists(fileDirectory))
                    {
                        Directory.CreateDirectory(fileDirectory);
                    }

                    var fileName = uploadFile.FileName;
                    var filePath = Path.Combine(fileDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadFile.File.CopyToAsync(stream);
                    }

                    return Ok(new { FilePath = filePath });
                }
                else
                {
                    return BadRequest("No file uploaded");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("readfile")]
        public IActionResult ReadFile([FromQuery] GetModel getFile)
        {
            try
            {
                var filePath = Path.Combine($"{_uploadDirectory}/{getFile.FilePath}", getFile.FileName);

                if (System.IO.File.Exists(filePath))
                {
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);

                    return File(fileBytes, "application/octet-stream", getFile.FileName);
                }
                else
                {
                    return NotFound("File not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
