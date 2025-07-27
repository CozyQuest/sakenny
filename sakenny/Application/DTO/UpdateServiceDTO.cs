using System.ComponentModel.DataAnnotations;

namespace sakenny.Application.DTO
{
    public class UpdateServiceDTO
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
