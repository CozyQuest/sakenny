using System.ComponentModel.DataAnnotations;

namespace sakenny.DAL.DTO
{
    public class MultiUploadImageRequestDTO
    {
        [Required]
        public List<IFormFile>? Files { get; set; }
    }
}
