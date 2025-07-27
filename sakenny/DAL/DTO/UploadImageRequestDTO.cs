using Microsoft.AspNetCore.Mvc;

namespace sakenny.DAL.DTO
{
    public class UploadImageRequestDTO
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
    }
}
