using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.Application.Services;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace sakenny.Services
{
    public class UserService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly LoginService _loginService;

        public UserService(IConfiguration configuration, IUnitOfWork unitOfWork, IImageService imageService, LoginService loginService, IMapper mapper)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _loginService = loginService;
        }

        public async Task<IdentityResult> registerUser(RegistrationDTO model)
        {
            var user = _mapper.Map<User>(model);
            var result = await _unitOfWork.userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return result;
            }
            var roleResult = await _unitOfWork.userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                await _unitOfWork.userManager.DeleteAsync(user);
                return roleResult;
            }
            return result;
        }

        // Updated method to return TokenResponseDTO
        public async Task<TokenResponseDTO> LoginUserAsync(LoginDTO model)
        {
            var token = await _loginService.LoginAsync(model);
            return token;
        }

        // Keep the old method for backward compatibility
        public async Task<string> LoginUser(LoginDTO model)
        {
            var tokenResponse = await LoginUserAsync(model);
            return tokenResponse.AccessToken;
        }

        public async Task<TokenResponseDTO> RefreshTokenAsync(RefreshTokenDTO model)
        {
            var token = await _loginService.RefreshTokenAsync(model);
            return token;
        }

        public async Task<IdentityResult> SetProfilePicAsync(String Id, UploadImageRequestDTO file)
        {
            if (file == null || file.File.Length == 0)
            {
                throw new InvalidOperationException("No file uploaded.");
            }
            var user = (User)await _unitOfWork.userManager.FindByIdAsync(Id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            try
            {
                var imageUrl = await _imageService.UploadImageAsync(file.File);
                user.UrlProfilePicture = imageUrl;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"File upload failed: {ex.Message}");
            }
            return await _unitOfWork.userManager.UpdateAsync(user);
        }
        public async Task<(Stream? Content, string? ContentType)> GetProfilePicAsync(string Id)
        {
            var user = (User)await _unitOfWork.userManager.FindByIdAsync(Id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            try
            {
                var (Content, ContentType) = await _imageService.GetImageStreamAsync(user.UrlProfilePicture);
                return (Content, ContentType);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Problem with fetching Image msg:{ex.Message}");
            }
            return (null, null);
        }

        public async Task<UserProfileDTO> GetUserPrivateProfileAsync(string userId)
        {
            var user = await _unitOfWork.userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            return _mapper.Map<UserProfileDTO>(user);

        }

        public async Task<UserPublicProfileDTO> GetUserPublicProfileAsync(string userId)
        {
            var user = await _unitOfWork.userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

             var dto = _mapper.Map<UserPublicProfileDTO>(user);

            var roles = await _unitOfWork.userManager.GetRolesAsync(user);
            dto.Role = roles.FirstOrDefault() ?? "User";

            return dto;
        }

        public async Task<IdentityResult> SetIdImagesAsync(String Id, BecomeHostRequest files)
        {
            if (files == null || files.BackImage.Length == 0 || files.FrontImage.Length == 0)
            {
                throw new InvalidOperationException("No file uploaded.");
            }
            var user = (User)await _unitOfWork.userManager.FindByIdAsync(Id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            if (user.HostRequest == true)
            {
                throw new InvalidOperationException("You Already requested once");
            }
            try
            {
                var FrontImageUrl = await _imageService.UploadImageAsync(files.FrontImage);
                user.UrlIdFront = FrontImageUrl;
                var BackImage = await _imageService.UploadImageAsync(files.BackImage);
                user.UrlIdBack = BackImage;
                user.HostRequest = true;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"File upload failed: {ex.Message}");
            }

            return await _unitOfWork.userManager.UpdateAsync(user);
        }
        public async Task<bool> CheckBecomeHostRequest(String Id)
        {
            var user = (User)await _unitOfWork.userManager.FindByIdAsync(Id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            return user.HostRequest;
        }
        public async Task<IdentityResult> ConvertToHost(String Id)
        {
            var user = (User)await _unitOfWork.userManager.FindByIdAsync(Id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            try
            {
                await _unitOfWork.userManager.RemoveFromRoleAsync(user, "User");
                var respond = await _unitOfWork.userManager.AddToRoleAsync(user, "Host");
                return respond;

            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<IdentityResult> DenyConvertToHost(String Id)
        {
            var user = (User)await _unitOfWork.userManager.FindByIdAsync(Id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            try
            {
                user.UrlIdBack = null;
                user.UrlIdFront = null;
                user.HostRequest = false;
                return await _unitOfWork.userManager.UpdateAsync(user);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public List<UserHostRequestDTO> GetAllUserRequest()
        {
            var users = _unitOfWork.userManager.Users
                .Where(u => ((User)u).HostRequest == true)
                .Cast<User>()
                .ToList();

            return _mapper.Map<List<UserHostRequestDTO>>(users);
        }

        public async Task<IdentityResult> SetIdImagesAsync(String Id, BecomeHostRequest files)
        {
            if (files == null || files.BackImage.Length == 0 || files.FrontImage.Length == 0)
            {
                throw new InvalidOperationException("No file uploaded.");
            }
            var user = (User)await _unitOfWork.userManager.FindByIdAsync(Id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            if (user.HostRequest == true)
            {
                throw new InvalidOperationException("You Already requested once");
            }
            try
            {
                var FrontImageUrl = await _imageService.UploadImageAsync(files.FrontImage);
                user.UrlIdFront = FrontImageUrl;
                var BackImage = await _imageService.UploadImageAsync(files.BackImage);
                user.UrlIdBack = BackImage;
                user.HostRequest = true;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"File upload failed: {ex.Message}");
            }

            return await _unitOfWork.userManager.UpdateAsync(user);
        }
        public async Task<bool> CheckBecomeHostRequest(String Id)
        {
            var user = (User)await _unitOfWork.userManager.FindByIdAsync(Id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            return user.HostRequest;
        }
        public async Task<IdentityResult> ConvertToHost(String Id)
        {
            var user = (User)await _unitOfWork.userManager.FindByIdAsync(Id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            try
            {
                await _unitOfWork.userManager.RemoveFromRoleAsync(user, "User");
                var respond = await _unitOfWork.userManager.AddToRoleAsync(user, "Host");
                return respond;

            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public async Task<IdentityResult> DenyConvertToHost(String Id)
        {
            var user = (User)await _unitOfWork.userManager.FindByIdAsync(Id);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            try
            {
                user.UrlIdBack = null;
                user.UrlIdFront = null;
                user.HostRequest = false;
                return await _unitOfWork.userManager.UpdateAsync(user);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public List<UserHostRequestDTO> GetAllUserRequest()
        {
            var users = _unitOfWork.userManager.Users
                .Where(u => ((User)u).HostRequest == true)
                .Cast<User>()
                .ToList();

            return _mapper.Map<List<UserHostRequestDTO>>(users);
        }

    }
}