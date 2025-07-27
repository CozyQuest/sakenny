using Microsoft.AspNetCore.Mvc;

namespace sakenny.Application.DTO
{
    public class UploadImageRequestDTO
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
    }
}
