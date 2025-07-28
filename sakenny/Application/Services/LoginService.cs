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
                var accessToken = await GenerateAccessTokenAsync(user);
                var refreshToken = GenerateRefreshToken();

                // Store refresh token in database
                await _unitOfWork.userManager.SetAuthenticationTokenAsync(user, "RefreshTokenProvider", "RefreshToken", refreshToken);

                var accessTokenExpiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!));
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpiryDays"]!));

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
        private async Task<string> GenerateAccessTokenAsync(IdentityUser user)
        {
            var userRoles = await _unitOfWork.userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
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
