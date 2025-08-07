using System.ComponentModel.DataAnnotations;

namespace sakenny.Application.DTO
{
    public class ForgetPasswordDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "The password must be at least {6} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
