using System.ComponentModel.DataAnnotations;

namespace sakenny.Application.DTO
{
    public class RegisterationDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UrlProfilePicture { get; set; }
    }

}
