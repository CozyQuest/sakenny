using System.ComponentModel.DataAnnotations;

namespace sakenny.Application.DTO
{
    public class AddTypeDTO
    {
        [Required(ErrorMessage = "Type name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; }
    }
}
