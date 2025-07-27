using System.ComponentModel.DataAnnotations;

namespace sakenny.Application.DTO
{
    public class LoginDTO
    {
        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        [Required(ErrorMessage = "Username is required.")]
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the remember me option for the user.
        /// </summary>
        public bool RememberMe { get; set; }
    }
}
