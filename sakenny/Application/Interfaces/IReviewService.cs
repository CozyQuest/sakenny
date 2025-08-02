using sakenny.Application.DTO;
using sakenny.DAL.Models;

namespace sakenny.Application.Interfaces
{
    public interface IReviewService
    {
        Task AddOrUpdateRatingAsync(int propertyId, PostRatingDTO dto, string userId);
        Task AddReviewAsync(int propertyId, PostReviewDTO dto, string userId);
        Task<IEnumerable<ReviewDTO>> GetReviewsForPropertyAsync(int propertyId);

    }
}
