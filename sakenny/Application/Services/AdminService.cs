using Microsoft.AspNetCore.Identity;
using sakenny.Application.DTO;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;

namespace sakenny.Application.Services
{
    public class AdminService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LoginService _loginService;

        public AdminService(
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            LoginService loginService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _loginService = loginService;
        }
        public async Task<IdentityResult> registerAdmin(AdminRegisterDTO model)
        {
            var user = new Admin
            {
                UserName = model.Username,
                Email = model.Email,
            };
            var result = await _unitOfWork.userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return result;
            }
            var succeded = await _unitOfWork.userManager.AddToRoleAsync(user, "Admin");
            if (!succeded.Succeeded)
            {
                await _unitOfWork.userManager.DeleteAsync(user);
                return succeded;
            }
            return result;
        }
        public async Task<TokenResponseDTO> LoginAdminAsync(LoginDTO model)
        {
            var token = await _loginService.LoginAsync(model);
            return token;
        }
        public async Task<TokenResponseDTO> RefreshTokenAsync(RefreshTokenDTO model)
        {
            var token = await _loginService.RefreshTokenAsync(model);
            return token;
        }
    }
}
