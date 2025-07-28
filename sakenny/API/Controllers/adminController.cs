using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sakenny.Application.DTO;
using sakenny.Application.Services;
using sakenny.DAL.Models;
using sakenny.Services;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class adminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public adminController(AdminService adminService )
        {
            _adminService = adminService;
        }
        [HttpPost("/AdminRegister")]
        public async Task<IActionResult> registerAdmin([FromBody] AdminRegisterDTO model)
        {
            var result = await _adminService.registerAdmin(model);
            if (result.Succeeded)
            {
                return Ok("Account Created Successfully");
            }
            else
            {
                return BadRequest(result.Errors);
            }
            return BadRequest("Could not create account");
        }
        // Updated login method to return TokenResponseDTO instead of just token
        [HttpPost("/AdminLogin")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var tokenResponse = await _adminService.LoginAdminAsync(model);
            if (!string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                return Ok(tokenResponse);
            }
            return Unauthorized(new { message = "Invalid Username or Password" });
        }

        // New refresh token endpoint
        [HttpPost("AdminRefresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO model)
        {
            var tokenResponse = await _adminService.RefreshTokenAsync(model);
            if (!string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                return Ok(tokenResponse);
            }
            return Unauthorized(new { message = "Invalid refresh token" });
        }
    }
}
