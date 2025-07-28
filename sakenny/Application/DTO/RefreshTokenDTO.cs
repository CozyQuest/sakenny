using System.ComponentModel.DataAnnotations;

namespace sakenny.Application.DTO
{
    public class RefreshTokenDTO
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
