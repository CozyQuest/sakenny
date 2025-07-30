using System.ComponentModel.DataAnnotations;

namespace sakenny.Application.DTO
{
    public class UpdateTypeDTO : AddTypeDTO
    {
        [Required(ErrorMessage = "Service ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ID must be greater than 0.")]
        public int Id { get; set; }
    }
}
