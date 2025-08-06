using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;

namespace sakenny.Application.Services
{
    public class UserUpdateDeleteProfileService : IUserUpdateDeleteProfile
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserUpdateDeleteProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task DeleteProfileAsync(string userId)
        {
            var user = (User)await _unitOfWork.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }
            var identityResult = await _unitOfWork.userManager.DeleteAsync(user);
            if (!identityResult.Succeeded)
            {
                throw new Exception("Failed to delete user profile");
            }
            var result = await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateUserProfileAsync(UserProfileDTO model)
        {
            var user = (User)await _unitOfWork.userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }
            user.FirstName = model.Fname;
            user.LastName = model.Lname;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.UrlProfilePicture = model.ProfilePicUrl;
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                throw new Exception("Failed to update user profile");
            }
                
        }
    }
}
