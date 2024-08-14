using Microsoft.AspNetCore.Http;

namespace DataTransmissionAPI.Dto.Files
{
    public class UploadModel
    {
        public required string FilePath { get; set; }
        public required string FileName { get; set; }
        public IFormFile File { get; set; }
    }
}
