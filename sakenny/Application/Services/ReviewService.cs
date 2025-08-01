using AutoMapper;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;

namespace sakenny.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddReviewAsync(int propertyId, PostReviewDTO model, string userId)
        {

            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property == null || property.IsDeleted)
                throw new KeyNotFoundException("Property not found.");

            var existingReview = (await _unitOfWork.Reviews
                .GetAllAsync(r => r.PropertyId == propertyId && r.UserId == userId))
                .FirstOrDefault();

            if (existingReview != null)
            {
                throw new InvalidOperationException("You have already submitted a review for this property.");
            }

            var review = new Review
            {
                PropertyId = propertyId,
                UserId = userId,
                ReviewText = model.ReviewText ?? "", 
                Rate = model.Rate > 0 ? model.Rate : 1,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();
        }

     

    }
}
