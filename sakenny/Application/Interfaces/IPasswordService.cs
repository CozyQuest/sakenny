using Microsoft.AspNetCore.Identity;
using sakenny.Application.DTO;

namespace sakenny.Application.Interfaces
{
    public interface IPasswordService
    {
        Task<IdentityResult> UpdatePasswordAsync(string userId, UpdatePasswordDTO updatePasswordDTO);
    }
}
