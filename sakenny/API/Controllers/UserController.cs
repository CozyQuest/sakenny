using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sakenny.Application.DTO;
using sakenny.Services;
using System.Security.Claims;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> register([FromBody] RegistrationDTO model)
        {
            if(model == null)
            {
                return BadRequest("Invalid registration data.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Returns validation errors
            }
            var result = await _userService.registerUser(model);
            if (result.Succeeded)
            {
                return Ok("Account Created Successfully");
            }
            return BadRequest(result.Errors.
                Select(result => result.Description).ToList());
        }

        [Authorize(Roles = "User")]
        [HttpPost("/CompleteRegister")]
        public async Task<IActionResult> CompleteRegister([FromBody] RegistrationDTO model)
        {
            return Ok();
        }

        // Updated login method to return TokenResponseDTO instead of just token
        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var tokenResponse = await _userService.LoginUserAsync(model);
            if (!string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                return Ok(tokenResponse);
            }
            return Unauthorized(new { message = "Invalid Username or Password" });
        }

        // New refresh token endpoint
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO model)
        {
            var tokenResponse = await _userService.RefreshTokenAsync(model);
            if (!string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                return Ok(tokenResponse);
            }
            return Unauthorized(new { message = "Invalid refresh token" });
        }
        [Authorize(Roles = "User")]
        [HttpPost("ProfilePic")]
        public async Task<IActionResult> SetProfilePic([FromForm] UploadImageRequestDTO file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            if (file == null || file.File.Length == 0)
                return BadRequest("No file uploaded.");
            try
            {
                var Result = await _userService.SetProfilePicAsync(userId, file);
                if (Result.Succeeded)
                {
                    return Ok("Profile picture updated successfully.");
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"File upload failed: {ex.Message}");
            }
            return BadRequest("File upload failed");
        }
        [Authorize(Roles = "User")]
        [HttpGet("ProfilePic")]
        public async Task<IActionResult> GetProfilePic()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var (content, contentType) = await _userService.GetProfilePicAsync(userId);

            if (content == null)
                return NotFound("Image not found.");

            return File(content, contentType ?? "application/octet-stream");
        }

    }
}