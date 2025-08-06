using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using sakenny.Application.DTO;
using sakenny.DAL.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace sakenny.Application.Services
{
    public class LoginService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        public LoginService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<TokenResponseDTO> LoginAsync(LoginDTO model)
        {
            var user = await _unitOfWork.userManager.FindByEmailAsync(model.Email);
            if (user != null && await _unitOfWork.userManager.CheckPasswordAsync(user, model.Password))
            {
                // Calculate expiry time for RememberMe
                var tokenExpiry = model.RememberMe 
                    ? DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:RememberMeExpiryMinutes"]!))
                    : DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!));
                
                var accessToken = await GenerateAccessTokenAsync(user, tokenExpiry);
                var refreshToken = GenerateRefreshToken();

                // Store refresh token in database
                await _unitOfWork.userManager.SetAuthenticationTokenAsync(user, "RefreshTokenProvider", "RefreshToken", refreshToken);

                // Use RememberMe to determine token expiry
                var accessTokenExpiry = model.RememberMe 
                    ? DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:RememberMeExpiryMinutes"]!))
                    : DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!));
                
                var refreshTokenExpiry = model.RememberMe
                    ? DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:RememberMeRefreshTokenExpiryDays"]!))
                    : DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpiryDays"]!));

                return new TokenResponseDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiry = accessTokenExpiry,
                    RefreshTokenExpiry = refreshTokenExpiry
                };
            }
            return new TokenResponseDTO();
        }
        public async Task<TokenResponseDTO> RefreshTokenAsync(RefreshTokenDTO model)
        {
            // Find user by refresh token
            var users = _unitOfWork.userManager.Users.ToList();
            IdentityUser? validUser = null;

            foreach (var user in users)
            {
                var storedToken = await _unitOfWork.userManager.GetAuthenticationTokenAsync(user, "RefreshTokenProvider", "RefreshToken");
                if (storedToken == model.RefreshToken)
                {
                    validUser = user;
                    break;
                }
            }

            if (validUser == null)
                return new TokenResponseDTO();

            // Remove old refresh token
            await _unitOfWork.userManager.RemoveAuthenticationTokenAsync(validUser, "RefreshTokenProvider", "RefreshToken");

            // Generate new tokens
            var newAccessToken = await GenerateAccessTokenAsync(validUser);
            var newRefreshToken = GenerateRefreshToken();

            // Store new refresh token
            await _unitOfWork.userManager.SetAuthenticationTokenAsync(validUser, "RefreshTokenProvider", "RefreshToken", newRefreshToken);

            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!));
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpiryDays"]!));

            return new TokenResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiry = accessTokenExpiry,
                RefreshTokenExpiry = refreshTokenExpiry
            };
        }
        private async Task<string> GenerateAccessTokenAsync(IdentityUser user, DateTime? customExpiry = null)
        {
            var userRoles = await _unitOfWork.userManager.GetRolesAsync(user);

            // Cast to custom User to access FirstName and LastName
            var customUser = user as sakenny.DAL.Models.User;
            
            // Construct full name, fallback to email if name is empty
            var fullName = "";
            if (customUser != null && !string.IsNullOrWhiteSpace(customUser.FirstName) && !string.IsNullOrWhiteSpace(customUser.LastName))
            {
                fullName = $"{customUser.FirstName} {customUser.LastName}".Trim();
            }
            
            var nameClaimValue = !string.IsNullOrWhiteSpace(fullName) ? fullName : (user.Email ?? "");

            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, nameClaimValue),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                expires: customExpiry ?? DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                claims: authClaims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
