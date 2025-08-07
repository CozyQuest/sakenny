using sakenny.Application.DTO;
using sakenny.Application.Services;

namespace sakenny.Application.Interfaces
{
    public interface IUserUpdateDeleteProfile
    {
        Task UpdateUserProfileAsync(UserProfileDTO model);
        Task DeleteProfileAsync(string userId);
    }
}
