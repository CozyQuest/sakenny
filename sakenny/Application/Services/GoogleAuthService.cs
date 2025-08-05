using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using sakenny.Application.DTO;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;
using System.Security.Cryptography;

namespace sakenny.Application.Services
{
    public class GoogleAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LoginService _loginService;

        public GoogleAuthService(IConfiguration configuration, IUnitOfWork unitOfWork, LoginService loginService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _loginService = loginService;
        }

        public async Task<TokenResponseDTO> GoogleSignInAsync(GoogleAuthDTO model)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _configuration["Google:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, settings);
                
                var user = await _unitOfWork.userManager.FindByEmailAsync(payload.Email);
                
                if (user == null)
                {
                    // Create new user if doesn't exist
                    user = new User
                    {
                        UserName = payload.Email,
                        Email = payload.Email,
                        FirstName = payload.GivenName ?? "",
                        LastName = payload.FamilyName ?? "",
                        EmailConfirmed = true,
                        UrlProfilePicture = payload.Picture ?? "", // Google profile picture
                        HostRequest = false,
                        // Optional fields are set to null by default
                        UrlIdFront = null,
                        UrlIdBack = null
                    };

                    // Create user without password (Google authenticated)
                    var result = await _unitOfWork.userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return new TokenResponseDTO(); // Failed to create user
                    }

                    // Add role with error handling (same as regular registration)
                    var roleResult = await _unitOfWork.userManager.AddToRoleAsync(user, "User");
                    if (!roleResult.Succeeded)
                    {
                        // If role assignment fails, delete the user (cleanup)
                        await _unitOfWork.userManager.DeleteAsync(user);
                        return new TokenResponseDTO();
                    }
                }

                // Generate JWT tokens using existing LoginService
                var accessToken = await GenerateAccessTokenAsync(user);
                var refreshToken = GenerateRefreshToken();

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
            catch (Exception)
            {
                return new TokenResponseDTO();
            }
        }

        private async Task<string> GenerateAccessTokenAsync(IdentityUser user)
        {
            // Create a temporary LoginDTO to use existing login service
            var loginDto = new LoginDTO { Email = user.Email!, Password = "" };
            var tempResponse = await _loginService.LoginAsync(loginDto);
            
            // Since we can't access the private method, we'll generate our own token
            // This replicates the LoginService logic
            var userRoles = await _unitOfWork.userManager.GetRolesAsync(user);

            var authClaims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Name, user.UserName ?? ""),
                new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id)
            };

            authClaims.AddRange(userRoles.Select(role => new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role)));

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                claims: authClaims,
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                    new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                    Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256)
            );

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
} 