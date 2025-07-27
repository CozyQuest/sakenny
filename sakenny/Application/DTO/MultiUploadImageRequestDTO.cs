using System.ComponentModel.DataAnnotations;

namespace sakenny.Application.DTO
{
    public class MultiUploadImageRequestDTO
    {
        [Required]
        public List<IFormFile>? Files { get; set; }
    }
}
