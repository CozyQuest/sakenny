using System.ComponentModel.DataAnnotations;

namespace sakenny.Application.DTO
{
    public class GoogleAuthDTO
    {
        [Required]
        public string IdToken { get; set; }
        public bool RememberMe { get; set; }
    }
} 