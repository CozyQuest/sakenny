using sakenny.Application.DTO;

namespace sakenny.Application.Interfaces
{
    public interface IReviewService
    {
        Task AddReviewAsync(int propertyId, PostReviewDTO model, string userId);

    }
}
