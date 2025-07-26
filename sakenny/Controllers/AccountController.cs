using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using sakenny.DAL;
using sakenny.DAL.DTO;
using sakenny.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace sakenny.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDBContext db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDBContext _db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            db = _db;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;

        }

        [HttpPost("/register")]
        public async Task<IActionResult> post([FromBody]RegisterationDTO model)
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UrlProfilePicture = model.UrlProfilePicture,
                PhoneNumber = model.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            await assignRole();

            var succeded = await _userManager.AddToRoleAsync(user, "User");


            if( succeded.Succeeded)
            {
                return Ok("Account Created Successfully");
            }
            return BadRequest("Couldnot create account");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Generate JWT token here (not implemented in this example)
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                };

                authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var token = new JwtSecurityToken(

                    issuer: _configuration["Jwt:Issuer"],
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                        SecurityAlgorithms.HmacSha256)
                );

                return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            return Unauthorized(new { message = "Invalid Username or Password" });
        }

        [HttpPost("/register/admin")]
        public async Task<IActionResult> postAdmin([FromBody] RegisterationDTO model)
        {
            var user = new Admin
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            await assignRole();

            var succeded = await _userManager.AddToRoleAsync(user, "Admin");



            if (succeded.Succeeded)
            {
                return Ok("Account Created Successfully");
            }
            return BadRequest("Couldnot create account");
        }


        private async Task<bool> assignRole()
        {
            var roleExists = await _roleManager.RoleExistsAsync("User");
            var adminExists = await _roleManager.RoleExistsAsync("Admin");
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
            if (!adminExists)
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            }
            return true;
        }
    }
}
