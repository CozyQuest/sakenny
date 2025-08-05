using AutoMapper;
using Microsoft.AspNetCore.Identity;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL.Interfaces;

namespace sakenny.Application.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PasswordService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IdentityResult> UpdatePasswordAsync(string userId, UpdatePasswordDTO updatePasswordDTO)
        {
            var user = await _unitOfWork.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} was not found.");
            }
            var passwordCheck = await _unitOfWork.userManager.CheckPasswordAsync(user, updatePasswordDTO.CurrentPassword);
            if (!passwordCheck)
            {
                throw new InvalidOperationException("Current password is incorrect.");
            }
            var result = await _unitOfWork.userManager.ChangePasswordAsync(user, updatePasswordDTO.CurrentPassword, updatePasswordDTO.NewPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            return result;
        }
    }
}
