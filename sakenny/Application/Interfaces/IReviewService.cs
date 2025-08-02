using sakenny.Application.DTO;
using sakenny.DAL.Models;

namespace sakenny.Application.Interfaces
{
    public interface IReviewService
    {
        Task AddReviewAsync(int propertyId, PostReviewDTO model, string userId);
        Task<IEnumerable<ReviewDTO>> GetReviewsForPropertyAsync(int propertyId);

    }
}
