using System.ComponentModel.DataAnnotations;

namespace sakenny.Application.DTO
{
    public class RegistrationDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [RegularExpression(@"^(?:\+20|0)?1[0125][0-9]{8}$", ErrorMessage = "Not a valid Egyptian phone number")]
        public string PhoneNumber { get; set; }

    }

}
